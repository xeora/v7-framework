using System.IO;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Net
{
    public class NetworkStream : Stream
    {
        private const int BUFFER_SIZE = 1024;
        private Stream _RemoteStream;

        private ConcurrentQueue<byte[]> _IncomeCache;
        private ConcurrentStack<byte[]> _Residual;

        private bool _Disposed;
        private Thread _StreamListenerThread;

        public NetworkStream(Stream remoteStream)
        {
            this._RemoteStream = remoteStream;

            this._IncomeCache = new ConcurrentQueue<byte[]>();
            this._Residual = new ConcurrentStack<byte[]>();

            this._StreamListenerThread = new Thread(this.StreamListener);
            this._StreamListenerThread.IsBackground = true;
            this._StreamListenerThread.Priority = ThreadPriority.BelowNormal;
            this._StreamListenerThread.Start();
        }

        private void StreamListener() 
        {
            byte[] buffer = new byte[NetworkStream.BUFFER_SIZE];

            do
            {
                try
                {
                    int rC = this._RemoteStream.Read(buffer, 0, buffer.Length);

                    if (rC == 0)
                        throw new IOException(this._RemoteStream.GetType().Name);

                    byte[] cache = new byte[rC];
                    System.Array.Copy(buffer, cache, rC);

                    this._IncomeCache.Enqueue(cache);
                }
                catch
                {
                    this._Disposed = true;

                    return;
                }
            } while (true);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int initialOffset = offset;

            offset = this.ConsumeResidual(buffer, offset, count);
            if (initialOffset + count > offset)
                offset = this.ConsumeIncomeCache(buffer, offset, (initialOffset + count) - offset);

            return offset - initialOffset;
        }

        public bool Listen(System.Func<byte[], int, bool> callback)
        {
            System.DateTime listenBegins = System.DateTime.Now;
            byte[] buffer = new byte[BUFFER_SIZE];
            bool result = true;

            do
            {
                // Mono Framework SslStream ReadTimeout bug fix.
                if (System.DateTime.Now.Subtract(listenBegins).TotalMilliseconds > this._RemoteStream.ReadTimeout)
                    throw new IOException(this._RemoteStream.GetType().Name);

                int count = this.Read(buffer, 0, buffer.Length);

                if (count > 0)
                    result = callback(buffer, count);
                else
                    Thread.Sleep(1);
            } while (result && !this._Disposed);

            return !result;
        }

        private int ConsumeResidual(byte[] buffer, int offset, int count)
        {
            while (!this._Residual.IsEmpty)
            {
                byte[] data;

                if (!this._Residual.TryPop(out data))
                    continue;

                if (data.Length >= count)
                {
                    System.Array.Copy(data, 0, buffer, offset, count);

                    this.Return(data, count, data.Length - count);

                    offset += count;

                    break;
                }

                System.Array.Copy(data, 0, buffer, offset, data.Length);

                offset += data.Length;
                count -= data.Length;
            }

            return offset;
        }

        private int ConsumeIncomeCache(byte[] buffer, int offset, int count)
        {
            while (!this._IncomeCache.IsEmpty)
            {
                byte[] data;

                if (!this._IncomeCache.TryDequeue(out data))
                    continue;

                if (data.Length >= count)
                {
                    System.Array.Copy(data, 0, buffer, offset, count);

                    this.Return(data, count, data.Length - count);

                    offset += count;

                    break;
                }

                System.Array.Copy(data, 0, buffer, offset, data.Length);

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
            System.Array.Copy(buffer, offset, stackData, 0, count);

            this._Residual.Push(stackData);
        }

        public override bool CanRead => this._RemoteStream.CanRead;
        public override bool CanSeek => this._RemoteStream.CanSeek;
        public override bool CanWrite => this._RemoteStream.CanWrite;
        public override long Length => this._RemoteStream.Length;
        public override long Position { get => this._RemoteStream.Position; set => this._RemoteStream.Position = value; }

        public override void Flush() =>
            this._RemoteStream.Flush();

        public override long Seek(long offset, SeekOrigin origin) =>
            this._RemoteStream.Seek(offset, origin);

        public override void SetLength(long value) =>
            this._RemoteStream.SetLength(value);

        public new void Dispose()
        {
            this._RemoteStream.Close();
            this._RemoteStream.Dispose();
        }
    }
}