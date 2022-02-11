using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class ResponseHandler
    {
        private class ResponseContainer : IDisposable
        {
            private const int MESSAGE_WAIT_DURATION = 30000; // 30 seconds

            private readonly object _Lock = new();
            private readonly MemoryStream _ContentStream;
            private readonly DateTime _CreatedAt;
            private bool _Concluded;
            
            public ResponseContainer(long requestId)
            {
                this.RequestId = requestId;
                this.MessageBlock = null;
                
                this._ContentStream = new MemoryStream();
                this._CreatedAt = DateTime.UtcNow;
            }

            public long RequestId { get; }
            public byte[] MessageBlock { get; private set; }
            
            public void Write(byte[] buffer, int offset, int count) =>
                this._ContentStream.Write(buffer, offset, count);

            public void Wait()
            {
                Monitor.Enter(this._Lock);
                try
                {
                    if (this._Concluded) return;
                    
                    Monitor.Wait(this._Lock, ResponseContainer.MESSAGE_WAIT_DURATION);
                }
                finally
                {
                    Monitor.Exit(this._Lock);
                }
            }

            public void Completed()
            {
                this._ContentStream.Seek(0, SeekOrigin.Begin);
                this.MessageBlock = 
                    this._ContentStream.ToArray();
                
                this.Dispose();
            }

            public void Failed() => this.Dispose();

            public bool Expired() =>
                DateTime.Compare(this._CreatedAt.AddMilliseconds(ResponseContainer.MESSAGE_WAIT_DURATION), DateTime.UtcNow) <= 0;

            public void Dispose()
            {
                this._ContentStream.Close();
                
                Monitor.Enter(this._Lock);
                try
                {
                    this._Concluded = true;

                    Monitor.Pulse(this._Lock);
                }
                finally
                {
                    Monitor.Exit(this._Lock);
                }
            }
        }
        
        private const int REQUEST_HEADER_LENGTH = 8; // 8 bytes first 5 bytes are requestId, remain 3 bytes are request length.
        private readonly TcpClient _DssServiceClient;

        private readonly object _AddRemoveLock = new();
        private readonly Dictionary<long, ResponseContainer> _ResponseResults;
        private readonly Action<Exception> _ErrorHandler;
        
        private readonly Thread _ResultCleanupThread;

        public ResponseHandler(ref TcpClient dssServiceClient, Action<Exception> errorHandler)
        {
            this._DssServiceClient = dssServiceClient;
            this._ResponseResults = new Dictionary<long, ResponseContainer>();
            this._ErrorHandler = errorHandler;
            
            this._ResultCleanupThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromHours(1));
                        this.CleanupResponseContainer();
                    }
                }
                catch 
                { /* Just Handle Exceptions */ }
            })
            {
                IsBackground = true, 
                Priority = ThreadPriority.Lowest
            };
            this._ResultCleanupThread.Start();
        }

        // Push handler to work in a different core using thread, It was async before.
        public void StartHandler() =>
            ThreadPool.QueueUserWorkItem(this.Handle);

        private ResponseContainer ProvideResponseContainer(long requestId)
        {
            ResponseContainer container =
                new ResponseContainer(requestId);
            Monitor.Enter(this._AddRemoveLock);
            try
            {
                if (this._ResponseResults.ContainsKey(requestId))
                    container = this._ResponseResults[requestId];
                else
                    this._ResponseResults[requestId] = container;
                
                return container;
            }
            finally
            {
                Monitor.Exit(this._AddRemoveLock);
            }
        }
        
        private void DropResponseContainer(long requestId)
        {
            Monitor.Enter(this._AddRemoveLock);
            try
            {
                if (!this._ResponseResults.ContainsKey(requestId)) return;
                this._ResponseResults.Remove(requestId);
            }
            finally
            {
                Monitor.Exit(this._AddRemoveLock);
            }
        }
        
        private void CleanupResponseContainer()
        {
            Monitor.Enter(this._AddRemoveLock);
            try
            {
                foreach (var (requestId, container) in this._ResponseResults)
                {
                    if (!container.Expired()) continue;
                    this._ResponseResults.Remove(requestId);
                }
            }
            finally
            {
                Monitor.Exit(this._AddRemoveLock);
            }
        }
        
        public byte[] WaitForMessage(long requestId)
        {
            ResponseContainer container =
                this.ProvideResponseContainer(requestId);
            
            container.Wait();

            this.DropResponseContainer(requestId);
            
            return container.MessageBlock;
        }
        
        private void Handle(object state)
        {
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
                this._ErrorHandler.Invoke(ex);
            }
            finally
            {
                this._ResultCleanupThread?.Interrupt();
            }
        }

        private void Consume(ref Stream responseStream, byte[] contentHead)
        {
            // 8 bytes first 5 bytes are requestId, remain 3 bytes are request length. Request length can be max 15Mb
            long head = BitConverter.ToInt64(contentHead, 0);

            long requestId = head >> 24;
            int contentSize = (int)(head & 0xFFFFFF);

            byte[] buffer = new byte[8192];

            ResponseContainer container =
                this.ProvideResponseContainer(requestId);
            try
            {
                while (contentSize > 0)
                {
                    int readLength = buffer.Length;
                    if (contentSize < readLength)
                        readLength = contentSize;

                    int bR = 
                        responseStream.Read(buffer, 0, readLength);

                    container.Write(buffer, 0, bR);

                    contentSize -= bR;
                }
                container.Completed();
            }
            catch
            {
                container.Failed();
                throw;
            }
        }
    }
}
