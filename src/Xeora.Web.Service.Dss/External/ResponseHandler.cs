using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class ResponseHandler
    {
        private readonly TcpClient _DssServiceClient;
        
        private readonly BlockingCollection<bool> _NotificationChannel = 
            new BlockingCollection<bool>();
        private readonly ConcurrentDictionary<long, byte[]> _ResponseResults;

        public ResponseHandler(ref TcpClient dssServiceClient)
        {
            this._DssServiceClient = dssServiceClient;
            this._ResponseResults = new ConcurrentDictionary<long, byte[]>();
        }

        // Push handler to work in a different core using thread, It was async before.
        public void StartHandler() =>
            ThreadPool.QueueUserWorkItem(this.Handle);

        public byte[] WaitForMessage(long requestId)
        {
            do
            {
                this._NotificationChannel.Take();
                if (this._ResponseResults.TryRemove(requestId, out byte[] message))
                    return message;
                this._NotificationChannel.Add(true);
            } while (true);
        }

        private void Handle(object state)
        {
            byte[] head = new byte[8];
            int bR = 0;

            Stream responseStream = this._DssServiceClient.GetStream();
            do
            {
                // Read Head
                bR += responseStream.Read(head, bR, head.Length - bR);
                if (bR == 0)
                {
                    this._DssServiceClient.Dispose();
                    return;
                }

                if (bR < 8)
                    continue;

                this.Consume(ref responseStream, head);

                bR = 0;
            } while (true);
        }

        private void Consume(ref Stream responseStream, byte[] contentHead)
        {
            // 8 bytes first 5 bytes are requestId, remain 3 bytes are request length. Request length can be max 15Mb
            long head = BitConverter.ToInt64(contentHead, 0);

            long requestId = head >> 24;
            int contentSize = (int)(head & 0xFFFFFF);

            byte[] buffer = new byte[8192];

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();

                while (contentSize > 0)
                {
                    int readLength = buffer.Length;
                    if (contentSize < readLength)
                        readLength = contentSize;

                    int bR = 
                        responseStream.Read(buffer, 0, readLength);

                    contentStream.Write(buffer, 0, bR);

                    contentSize -= bR;
                }

                byte[] messageBlock = ((MemoryStream)contentStream).ToArray();
                this._ResponseResults.TryAdd(requestId, messageBlock);
            }
            catch
            {
                this._ResponseResults.TryAdd(requestId, null);
            }
            finally
            {
                contentStream?.Close();
                
                this._NotificationChannel.Add(true);
            }
        }
    }
}
