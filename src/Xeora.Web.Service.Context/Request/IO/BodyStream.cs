using System;
using System.Collections.Generic;
using System.IO;

namespace Xeora.Web.Service.Context.Request.IO
{
    public abstract class BodyStream : Stream
    {
        private readonly Net.NetworkStream _UnderlyingStreamEnclosure;
        private readonly Stream _LoopbackStream;

        protected const int BUFFER_SIZE = short.MaxValue;
        
        protected BodyStream() {}
        protected BodyStream(Net.NetworkStream streamEnclosure)
        {
            this._UnderlyingStreamEnclosure = streamEnclosure;
            
            this._LoopbackStream = new Net.LoopbackStream(streamEnclosure.ReadTimeout, streamEnclosure.WriteTimeout);
            this.StreamEnclosure = new Net.NetworkStream(ref this._LoopbackStream);
            
            this._UnderlyingStreamEnclosure.Pipe(
                buffer => this._LoopbackStream.Write(buffer, 0, buffer.Length));
        }

        protected Net.NetworkStream StreamEnclosure { get; }
        
        internal abstract ParserResultTypes ReadAllInto(ref Stream contentStream);
        public abstract void Conclude();

        protected int ReadWithLength(byte[] buffer, int offset, int count)
        {
            int innerRead = 0;
            
            this.StreamEnclosure.Listen((streamBuffer, size) =>
            {
                int read = size;
                if (count < read)
                    read = count;
                innerRead += read;
                
                Array.Copy(streamBuffer, 0, buffer, offset, read);
                offset += read;
                count -= read;

                if (read - size < 0)
                    this.StreamEnclosure.Return(streamBuffer, read, size - read);

                return count > 0;
            });

            return innerRead;
        }

        private void ReturnToSource()
        {
            Stack<byte[]> returnStack = new Stack<byte[]>();
            byte[] returningBytes = new byte[BUFFER_SIZE];
            
            do
            {
                int bC = 
                    this.StreamEnclosure.Read(returningBytes, 0, returningBytes.Length);
                if (bC == 0) break;

                byte[] cache = new byte[bC];
                Array.Copy(returningBytes, 0, cache, 0, bC);
                
                returnStack.Push(cache);
            } while (true);
            
            while (returnStack.TryPop(out byte[] stackedCache))
                this._UnderlyingStreamEnclosure.Return(stackedCache, 0, stackedCache.Length);
        }

        public new void Dispose()
        {
            if (this._UnderlyingStreamEnclosure == null) return;
            
            this._UnderlyingStreamEnclosure.Pipe(null);
            this.Conclude();
            this.ReturnToSource();
            ((Net.LoopbackStream)this._LoopbackStream).Dispose();
        }
    }
}
