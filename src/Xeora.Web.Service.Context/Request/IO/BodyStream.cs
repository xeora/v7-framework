using System;
using System.IO;

namespace Xeora.Web.Service.Context.Request.IO
{
    public abstract class BodyStream : Stream
    {
        protected const int BUFFER_SIZE = short.MaxValue;
        
        protected BodyStream() {}
        protected BodyStream(Net.NetworkStream streamEnclosure) =>
            this.StreamEnclosure = streamEnclosure;

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

        public new void Dispose() =>
            this.Conclude();
    }
}
