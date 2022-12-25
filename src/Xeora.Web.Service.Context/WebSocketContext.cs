using System;
using System.IO;
using System.Text;
using Xeora.Web.Basics.Context;
using Xeora.Web.Service.Context.WebSocket;
using NetworkStream = Xeora.Web.Service.Net.NetworkStream;

namespace Xeora.Web.Service.Context
{
    public class WebSocketContext : IWebSocketContext
    {
        /*
         * 9000: Handshake Unsuccessful
         * 9001: Operational Error
         */
        private const string COMPRESSION_EXTENSION_NAME = "permessage-deflate";
        private const string MAGIC_KEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        private readonly string _FrameworkVersion;
        private readonly NetworkStream _StreamEnclosure;
        private readonly bool _EnableCompression;
        
        public event IWebSocketContext.OnOpenedHandler OnOpened;
        public event IWebSocketContext.OnErrorHandler OnError;
        public event IWebSocketContext.OnMessageHandler OnMessage;
        public event IWebSocketContext.OnClosedHandler OnClosed;
        
        public WebSocketContext(
            string contextId,
            IWebSocketRequest request,
            string frameworkVersion,
            NetworkStream streamEnclosure)
        {
            this.UniqueId = contextId;
            this.Request = request;
            this._FrameworkVersion = frameworkVersion;
            this._StreamEnclosure = streamEnclosure;
            this._EnableCompression =
                request.Header.Extensions.IndexOf(
                    COMPRESSION_EXTENSION_NAME, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        public string UniqueId { get; }
        public IWebSocketRequest Request { get; }
        
        private string CreateResponseAcceptKey(string requestKey)
        {
            string responseKey = $"{requestKey}{MAGIC_KEY}";
            
            System.Security.Cryptography.SHA1 digest =
                System.Security.Cryptography.SHA1.Create();

            byte[] encryptedKey = 
                digest.ComputeHash(Encoding.UTF8.GetBytes(responseKey));
            return Convert.ToBase64String(encryptedKey);
        }
        
        private bool PushWebSocketAcceptance()
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.AppendFormat("HTTP/1.1 101 Switching Protocols");
                sB.Append(HttpResponse.Newline);
                sB.Append("Upgrade: websocket");
                sB.Append(HttpResponse.Newline);
                sB.Append("Connection: Upgrade");
                sB.Append(HttpResponse.Newline);
                if (this._EnableCompression)
                {
                    sB.Append($"Sec-WebSocket-Extensions: {COMPRESSION_EXTENSION_NAME}; client_no_context_takeover");
                    sB.Append(HttpResponse.Newline);
                }
                sB.AppendFormat("Sec-WebSocket-Accept: {0}", this.CreateResponseAcceptKey(this.Request.Header.Key));
                sB.Append(HttpResponse.Newline);
                sB.Append("Server: XeoraEngine");
                sB.Append(HttpResponse.Newline);
                sB.Append("X-Powered-By: Xeora");
                sB.Append(HttpResponse.Newline);
                sB.Append($"X-Framework-Version: {this._FrameworkVersion}");
                sB.Append(HttpResponse.Newline);
                sB.Append(HttpResponse.Newline);

                byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
                this._StreamEnclosure.Write(buffer, 0, buffer.Length);
                this._StreamEnclosure.KeepAlive = false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Start()
        {
            this._StreamEnclosure.BumpToWebSocket();
            
            bool result = this.PushWebSocketAcceptance();
            if (!result)
            {
                this.OnError?.Invoke(9000, "Handshake Unsuccessful");
                this.OnClosed?.Invoke(1011);
                return;
            }
            
            this.OnOpened?.Invoke();
            
            short statusCode = 1000;
            try
            {
                do
                {
                    MemoryStream contentStream = null;
                    try
                    {
                        contentStream = new MemoryStream();

                        Frame frame = new Frame(size =>
                        {
                            byte[] buffer = new byte[size];
                            this.ReadAtLeast(buffer, 0, buffer.Length);
                            return buffer;
                        });
                        
                        byte[] frameHeader = new byte[2];
                        this.ReadAtLeast(frameHeader, 0, frameHeader.Length);
                        
                        frame.Inject(frameHeader);
                        
                        if (!frame.Mask)
                        {
                            // Protocol Error (1002)
                            this.PushCloseControlFrameWithError(1002);
                            return;
                        }

                        frame.ProcessInto(contentStream);
                        
                        switch (frame.OpCode)
                        {
                            case OpCodes.Continue:
                                // it is the sequence data frame (ignore)
                                break;
                            case OpCodes.Text:
                                // text data frame
                                this.OnMessage?.Invoke(
                                    new WebSocketMessage(
                                        WebSocketMessageTypes.Text, 
                                        WebSocketContext.GetBuffer(contentStream)
                                    )
                                );
                                
                                break;
                            case OpCodes.Binary:
                                // binary data frame
                                this.OnMessage?.Invoke(
                                    new WebSocketMessage(
                                        WebSocketMessageTypes.Binary, 
                                        WebSocketContext.GetBuffer(contentStream)
                                    )
                                );
                                
                                break;
                            case OpCodes.Close:
                                // closing connection
                                byte[] statusCodeBytes =
                                    WebSocketContext.GetBuffer(contentStream);
                                Array.Resize(ref statusCodeBytes, 2);
                                Array.Reverse(statusCodeBytes);
                                statusCode = BitConverter.ToInt16(statusCodeBytes);
                                
                                return;
                            case OpCodes.Ping:
                                // ping (requires pong)
                                this.SendPong(WebSocketContext.GetBuffer(contentStream));
                                continue;
                            case OpCodes.Pong:
                                // pong (not server's responsibility)
                                break;
                        }
                    }
                    finally
                    {
                        contentStream?.Dispose();
                    }
                } while (true);
            }
            catch (IOException e)
            {
                statusCode = 1011;
                this.PushCloseControlFrameWithError(1011);
                this.OnError?.Invoke(9001, $"Operational Error: {e.Message}");
            }
            finally
            {
                this.OnClosed?.Invoke(statusCode);
            }
        }

        public void Send(string message)
        {
            byte[] buffer = 
                Encoding.UTF8.GetBytes(message);

            Frame frame = 
                new Frame
                {
                    Fin = true,
                    OpCode = OpCodes.Text
                };
            frame.BuildInto(buffer, 0, buffer.Length, this._StreamEnclosure);
        }
        
        public void Send(byte[] buffer, int offset, int count)
        {
            Frame frame = 
                new Frame
                {
                    Fin = true,
                    OpCode = OpCodes.Binary
                };
            frame.BuildInto(buffer, offset, count, this._StreamEnclosure);
        }

        private void ReadAtLeast(byte[] buffer, int offset, int count)
        {
            int readUntil = offset + count;
            
            bool result = 
                this._StreamEnclosure.Listen((b, size) =>
                {
                    int targetCount = count;
                    if (size < targetCount) targetCount = size;
                    
                    Array.Copy(b, 0, buffer, offset, targetCount);
                    offset += targetCount;
                    count -= targetCount;
                    
                    this._StreamEnclosure.Return(b, targetCount, size - targetCount);

                    return readUntil != offset;
                });
            if (!result) throw new IOException();
        }
        
        private static byte[] GetBuffer(Stream contentStream)
        {
            byte[] buffer = 
                new byte[contentStream.Position];
            
            contentStream.Seek(buffer.Length * -1, SeekOrigin.Current);
            var _ = contentStream.Read(buffer, 0, buffer.Length);

            return buffer;
        }
        
        private void SendPong(byte[] dataBytes)
        {
            Frame frame = 
                new Frame
                {
                    Fin = true,
                    OpCode = OpCodes.Pong
                };
            
            frame.BuildInto(dataBytes, 0, dataBytes.Length, this._StreamEnclosure);
        }
        
        private void PushCloseControlFrameWithError(short errorCode)
        {
            Frame frame = 
                new Frame
                {
                    Fin = true,
                    OpCode = OpCodes.Close
                };
            
            byte[] errorCodeBytes =
                BitConverter.GetBytes(errorCode);
            Array.Reverse(errorCodeBytes);
            
            frame.BuildInto(errorCodeBytes, 0, errorCodeBytes.Length, this._StreamEnclosure);
        }
    }
}
