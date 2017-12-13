using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xeora.Web.External.Service.Session
{
    public class ConnectionHandler
    {
        private TcpClient _RemoteClient;
        private Stream _RemoteStream;
        private MessageHandler _MessageHandler;

        public ConnectionHandler(ref TcpClient remoteClient)
        {
            this._RemoteClient = remoteClient;
            this._RemoteStream = this._RemoteClient.GetStream();
            this._MessageHandler = new MessageHandler(ref this._RemoteStream);
        }

        public async void HandleAsync()
        {
            await Task.Run(() => this.ReadSocket());

            this._RemoteClient.Close();
        }

        private void ReadSocket()
        {
            byte[] head = new byte[8];
            byte[] buffer = new byte[1024];
            int bR = 0;

            try
            {
                do
                {
                    // Read Head
                    bR += this._RemoteStream.Read(head, bR, head.Length - bR);
                    if (bR == 0)
                    {
                        // give time to fill buffer
                        System.Threading.Thread.Sleep(1);

                        continue;
                    }

                    if (bR == 1 && buffer[0] == 0)
                        return;

                    if (bR < 8)
                        continue;

                    this.Consume(head);

                    bR = 0;
                } while (true);
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Basics.Console.Push("SYSTEM ERROR", ex.Message, false);
            }
        }

        private void Consume(byte[] contentHead)
        {
            // 8 bytes first 5 bytes are requestID, remain 3 bytes are request length. Request length can be max 15Mb
            long head = BitConverter.ToInt64(contentHead, 0);

            long requestID = head >> 24;
            int contentSize = (int)(head & 0xFFFFFF);

            byte[] buffer = new byte[1024];
            int bR = 0;

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();
                do
                {
                    int readLength = buffer.Length;
                    if (contentSize < readLength)
                        readLength = contentSize;
            
                    bR = this._RemoteStream.Read(buffer, 0, readLength);

                    contentStream.Write(buffer, 0, bR);

                    contentSize -= bR;
                } while (contentSize > 0);

                byte[] messageBlock = ((MemoryStream)contentStream).ToArray();
                this._MessageHandler.ProcessAsync(requestID, messageBlock);
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    GC.SuppressFinalize(contentStream);
                }
            }
        }
    }
}
