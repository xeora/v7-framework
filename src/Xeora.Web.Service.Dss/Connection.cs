using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Xeora.Web.Service.Dss
{
    public class Connection
    {
        private readonly TcpClient _RemoteClient;
        private readonly IManager _Manager;
        private Handler _Handler;

        public Connection(ref TcpClient remoteClient, IManager manager)
        {
            this._RemoteClient = remoteClient;
            this._Manager = manager;
        }

        public void Process()
        {
            Stream remoteStream = 
                this._RemoteClient.GetStream();
            
            this._Handler = 
                new Handler(ref remoteStream, this._Manager);
            
            if (this.Validate(ref remoteStream))
                this.ReadSocket(ref remoteStream);
            
            remoteStream.Close();
            remoteStream.Dispose();
            
            this._RemoteClient.Close();
            this._RemoteClient.Dispose();
            
            Basics.Console.Flush();
        }

        private bool Validate(ref Stream remoteStream)
        {
            this._RemoteClient.ReceiveTimeout = 30000; // 30 seconds
            
            byte[] code = 
                Guid.NewGuid().ToByteArray();
            byte[] response = new byte[code.Length];
            int total = 0;
            
            try
            {
                remoteStream.WriteByte((byte)code.Length);
                remoteStream.Write(code, 0, code.Length);

                do
                {
                    int bR = remoteStream.Read(response, total, response.Length - total);
                    if (bR == 0) return false;
                    
                    total += bR;
                } while (total < response.Length);

                bool correct =
                    code.SequenceEqual(response);
                
                remoteStream.WriteByte(correct ? (byte)1 : (byte)0);

                return correct;
            }
            catch (Exception ex)
            {
                // Skip SocketExceptions
                if (ex is not IOException || ex.InnerException is not SocketException)
                    Basics.Console.Push("SYSTEM ERROR", ex.Message, ex.ToString(), false, true, type: Basics.Console.Type.Error);

                return false;
            }
        }
        
        private void ReadSocket(ref Stream remoteStream)
        {
            this._RemoteClient.ReceiveTimeout = 0; // Infinite
            
            byte[] head = new byte[8];
            byte[] buffer = new byte[1024];
            int bR = 0;

            try
            {
                do
                {
                    // Read Head
                    bR += remoteStream.Read(head, bR, head.Length - bR);
                    if (bR == 0 || bR == 1 && buffer[0] == 0) return;
                    if (bR < 8) continue;

                    this.Consume(head, ref remoteStream);

                    bR = 0;
                } while (true);
            }
            catch (Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Basics.Console.Push("SYSTEM ERROR", ex.Message, ex.ToString(), false, true, type: Basics.Console.Type.Error);
            }
        }

        private void Consume(byte[] contentHead, ref Stream remoteStream)
        {
            // 8 bytes first 5 bytes are requestId, remain 3 bytes are request length. Request length can be max 15Mb
            long head = 
                BitConverter.ToInt64(contentHead, 0);

            long requestId = head >> 24;
            int contentSize = 
                (int)(head & 0xFFFFFF);

            byte[] buffer = new byte[1024];

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();
                do
                {
                    int readLength = buffer.Length;
                    if (contentSize < readLength)
                        readLength = contentSize;
            
                    int bR = 
                        remoteStream.Read(buffer, 0, readLength);

                    contentStream.Write(buffer, 0, bR);

                    contentSize -= bR;
                } while (contentSize > 0);

                byte[] messageBlock = 
                    ((MemoryStream)contentStream).ToArray();
                this._Handler.ProcessAsync(requestId, messageBlock);
            }
            finally
            {
                contentStream?.Close();
            }
        }
    }
}
