using System;
using System.IO;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Net
{
    public class NetworkStream : Stream
    {
        private const int BUFFER_SIZE = 2048;
        private readonly Stream _RemoteStream;

        private readonly ConcurrentQueue<byte[]> _IncomeCache;
        private readonly ConcurrentStack<byte[]> _Residual;

        private Action<byte[]> _FlowTarget;
        private readonly object _FlowTargetLock = new ();

        public NetworkStream(ref Stream remoteStream)
        {
            this._RemoteStream = remoteStream;

            this._IncomeCache = new ConcurrentQueue<byte[]>();
            this._Residual = new ConcurrentStack<byte[]>();
            this.KeepAlive = false;
            
            Thread streamListenerThread = new Thread(this.StreamListener)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            streamListenerThread.Start();
        }

        private void StreamListener()
        {
            SpinWait spinWait = new SpinWait();
            byte[] buffer = 
                new byte[NetworkStream.BUFFER_SIZE];

            do
            {
                try
                {
                    int rC = 
                        this._RemoteStream.Read(buffer, 0, buffer.Length);
                    if (rC == 0)
                    {
                        spinWait.SpinOnce();
                        continue;
                    }
                    
                    byte[] cache = 
                        new byte[rC];
                    Array.Copy(buffer, cache, cache.Length);
                    
                    if (!this.TryPipe(cache))
                        this._IncomeCache.Enqueue(cache);
                }
                catch
                {
                    this.Disposed = true;
                    return;
                }
            } while (true);
        }

        private bool TryPipeUnsafe(byte[] cache = null)
        {
            if (this._FlowTarget == null) return false;
                
            while (this._Residual.TryPop(out byte[] buffer))
                this._FlowTarget.Invoke(buffer);
                
            while (this._IncomeCache.TryDequeue(out byte[] buffer))
                this._FlowTarget.Invoke(buffer);
                    
            if (cache != null) this._FlowTarget.Invoke(cache);
            return true;
        }
        private bool TryPipe(byte[] cache = null)
        {
            Monitor.Enter(this._FlowTargetLock);
            try
            {
                return this.TryPipeUnsafe(cache);
            }
            finally
            {
                Monitor.Exit(this._FlowTargetLock);
            }
        }
        
        public void Pipe(Action<byte[]> callback)
        {
            Monitor.Enter(this._FlowTargetLock);
            try
            {
                this._FlowTarget = callback;
                this.TryPipeUnsafe();
            }
            finally
            {
                Monitor.Exit(this._FlowTargetLock);
            }
        }

        public bool Alive()
        {
            if (!this.KeepAlive) return false;
            return !this.Disposed;
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            int initialOffset = offset;

            offset = this.ConsumeResidual(buffer, offset, count);
            if (initialOffset + count > offset)
                offset = this.ConsumeIncomeCache(buffer, offset, initialOffset + count - offset);

            return offset - initialOffset;
        }

        public bool Listen(Func<byte[], int, bool> callback)
        {
            SpinWait spinWait = new SpinWait();
            DateTime listenBegins = 
                DateTime.Now;
            byte[] buffer = 
                new byte[BUFFER_SIZE];
            bool result = true;

            do
            {
                if (DateTime.Now.Subtract(listenBegins).TotalMilliseconds > this._RemoteStream.ReadTimeout)
                    throw new IOException("NetworkStream has a connection timeout", new SocketException());

                int count = 
                    this.Read(buffer, 0, buffer.Length);
                if (count == 0)
                {
                    if (this.Disposed) break;
                    
                    spinWait.SpinOnce();
                    continue;
                }

                result = callback(buffer, count);
                listenBegins = DateTime.Now;
            } while (result);

            return !result;
        }

        private int ConsumeResidual(byte[] buffer, int offset, int count)
        {
            while (!this._Residual.IsEmpty)
            {
                if (!this._Residual.TryPop(out byte[] data))
                    continue;

                if (data.Length >= count)
                {
                    Array.Copy(data, 0, buffer, offset, count);

                    this.Return(data, count, data.Length - count);

                    offset += count;

                    break;
                }

                Array.Copy(data, 0, buffer, offset, data.Length);

                offset += data.Length;
                count -= data.Length;
            }

            return offset;
        }

        private int ConsumeIncomeCache(byte[] buffer, int offset, int count)
        {
            while (!this._IncomeCache.IsEmpty)
            {
                if (!this._IncomeCache.TryDequeue(out byte[] data))
                    continue;

                if (data.Length >= count)
                {
                    Array.Copy(data, 0, buffer, offset, count);

                    this.Return(data, count, data.Length - count);

                    offset += count;

                    break;
                }

                Array.Copy(data, 0, buffer, offset, data.Length);

                offset += data.Length;
                count -= data.Length;
            }

            return offset;
        }

        public override void Write(byte[] buffer, int offset, int count) =>
            this._RemoteStream.Write(buffer, offset, count);
        
        public void Return(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;

            byte[] stackData = new byte[count];
            Array.Copy(buffer, offset, stackData, 0, count);

            this._Residual.Push(stackData);
        }

        public void Throw(int byteSize)
        {
            byte[] thrownBytes = new byte[byteSize];
            int thrownSize = this.Read(thrownBytes, 0, thrownBytes.Length);
            if (thrownSize != byteSize)
                throw new IOException("Requested thrown size is not matching with the thrown actually happened");
        }

        public bool KeepAlive { get; set; }
        public bool Disposed { get; private set; }
        public override bool CanRead => this._RemoteStream.CanRead;
        public override bool CanSeek => this._RemoteStream.CanSeek;
        public override bool CanWrite => this._RemoteStream.CanWrite;
        public override long Length => this._RemoteStream.Length;
        public override long Position { get => this._RemoteStream.Position; set => this._RemoteStream.Position = value; }

        public override int ReadTimeout => this._RemoteStream.ReadTimeout;
        public override int WriteTimeout => this._RemoteStream.WriteTimeout;
        
        public override void Flush() =>
            this._RemoteStream.Flush();

        public override long Seek(long offset, SeekOrigin origin) =>
            this._RemoteStream.Seek(offset, origin);

        public override void SetLength(long value) =>
            this._RemoteStream.SetLength(value);
    }
}