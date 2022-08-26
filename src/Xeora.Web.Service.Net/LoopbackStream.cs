using System;
using System.Collections.Concurrent;
using System.IO;

namespace Xeora.Web.Service.Net
{
    public class LoopbackStream : Stream
    {
        private readonly int _ReadTimeout;
        private readonly int _WriteTimeout;
        
        private readonly ConcurrentQueue<byte[]> _IncomeCache;
        private readonly ConcurrentStack<byte[]> _Residual;

        public LoopbackStream(int readTimeout, int writeTimeout)
        {
            this._ReadTimeout = readTimeout;
            this._WriteTimeout = writeTimeout;
            
            this._IncomeCache = new ConcurrentQueue<byte[]>();
            this._Residual = new ConcurrentStack<byte[]>();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.Disposed)
                throw new IOException("Write to the stream is not possible because it is disposed!");
            
            byte[] cache =
                new byte[count];
            Array.Copy(buffer, offset, cache, 0, count);
            this._IncomeCache.Enqueue(cache);
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Disposed)
                throw new IOException("Read from the stream is not possible because it is disposed!");
            
            int initialOffset = offset;

            offset = this.ConsumeResidual(buffer, offset, count);
            if (initialOffset + count > offset)
                offset = this.ConsumeIncomeCache(buffer, offset, initialOffset + count - offset);

            return offset - initialOffset;
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
        
        private void Return(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;

            byte[] stackData = new byte[count];
            Array.Copy(buffer, offset, stackData, 0, count);

            this._Residual.Push(stackData);
        }
        
        public override void Flush() =>
            throw new NotSupportedException();
        
        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            throw new NotSupportedException();
        
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override int ReadTimeout => this._ReadTimeout;
        public override int WriteTimeout => this._WriteTimeout;
        
        public override long Length =>
            throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public bool Disposed { get; private set; }
        public new void Dispose() => this.Disposed = true;
    }
}
