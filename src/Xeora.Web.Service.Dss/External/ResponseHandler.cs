using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class ResponseHandler
    {
        private const int REQUEST_HEADER_LENGTH = 8; // 8 bytes first 5 bytes are requestId, remain 3 bytes are request length.
        private readonly TcpClient _DssServiceClient;
        
        private const int MESSAGE_WAIT_DURATION = 30; // 30 seconds
        
        private readonly BlockingCollection<DateTime> _NotificationChannel = new();
        private readonly ConcurrentDictionary<long, byte[]> _ResponseResults;
        private readonly Action<Exception> _ErrorHandler;

        private bool _Running;

        public ResponseHandler(ref TcpClient dssServiceClient, Action<Exception> errorHandler)
        {
            this._DssServiceClient = dssServiceClient;
            this._ResponseResults = new ConcurrentDictionary<long, byte[]>();
            this._ErrorHandler = errorHandler;
        }

        // Push handler to work in a different core using thread, It was async before.
        public void StartHandler() =>
            ThreadPool.QueueUserWorkItem(this.Handle);

        public byte[] WaitForMessage(long requestId)
        {
            while (this._Running)
            {
                DateTime requestTime =
                    this._NotificationChannel.Take();
                if (this._ResponseResults.TryRemove(requestId, out byte[] message))
                    return message;

                if (DateTime.Compare(requestTime, DateTime.UtcNow) < 0)
                    return null;
                this._NotificationChannel.Add(requestTime);
            }
            return null;
        }

        private void Handle(object state)
        {
            this._Running = true;
            
            byte[] head = new byte[8];
            int bR = 0;

            try
            {
                Stream responseStream = this._DssServiceClient.GetStream();
                do
                {
                    // Read Head
                    bR += responseStream.Read(head, bR, head.Length - bR);
                    if (bR == 0) return;

                    if (bR < ResponseHandler.REQUEST_HEADER_LENGTH)
                        continue;

                    this.Consume(ref responseStream, head);

                    bR = 0;
                } while (true);
            }
            catch (Exception ex)
            {
                this._Running = false;
                this._ErrorHandler.Invoke(ex);
            }
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
                throw;
            }
            finally
            {
                contentStream?.Close();
                
                this._NotificationChannel.Add(
                    DateTime.UtcNow.AddSeconds(ResponseHandler.MESSAGE_WAIT_DURATION)
                );
            }
        }
    }
}
