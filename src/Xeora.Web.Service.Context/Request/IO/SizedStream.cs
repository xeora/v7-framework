using System;
using System.IO;

namespace Xeora.Web.Service.Context.Request.IO
{
    public class SizedStream : BodyStream
    {
        private readonly int _ContentLength;
        private int _ContentLengthCountdown;
        private bool _Concluded;

        private byte[] _Leftover = Array.Empty<byte>();
        
        public SizedStream(Net.NetworkStream streamEnclosure, int contentLength) :
            base(streamEnclosure)
        {
            this._ContentLength = contentLength;
            this._ContentLengthCountdown = contentLength;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bR = this.ServeLeftOver(buffer, offset, count);
            if (bR > 0) return bR;
            
            if (this._ContentLengthCountdown < count)
                count = this._ContentLengthCountdown;
            if (count == 0) return 0;
                
            int read = this.ReadWithLength(buffer, offset, count);
            this._ContentLengthCountdown -= read;
            return read;
        }

        internal override ParserResultTypes ReadAllInto(ref Stream contentStream)
        {
            contentStream ??= new MemoryStream();
            
            byte[] buffer = new byte[BUFFER_SIZE];
            
            do
            {
                if (this._Leftover.Length + this.Position == this._ContentLength)
                    return ParserResultTypes.Success;
                
                int read = 
                    this.Read(buffer, 0, buffer.Length);
                contentStream.Write(buffer, 0, read);
            } while (this._ContentLengthCountdown > 0);

            return this._ContentLengthCountdown == 0 
                ? ParserResultTypes.Success 
                : ParserResultTypes.BadRequest;
        }

        public override void Conclude()
        {
            if (this._Concluded) return;
            this._Concluded = true;
            
            Stream cacheContent = null;
            try
            {
                ParserResultTypes parserResult =
                    this.ReadAllInto(ref cacheContent);
                if (parserResult != ParserResultTypes.Success) return;

                this._Leftover =
                    ((MemoryStream)cacheContent).GetBuffer();
                this._ContentLengthCountdown = this._Leftover.Length;
            }
            finally
            {
                cacheContent?.Dispose();
            }
        }

        private int ServeLeftOver(byte[] buffer, int offset, int count)
        {
            if (this._Leftover.Length == 0) return 0;

            if (this._Leftover.Length > count)
            {
                Array.Copy(this._Leftover, 0, buffer, offset, count);
                Array.Copy(this._Leftover, count, this._Leftover, 0, this._Leftover.Length - count);
                Array.Resize(ref this._Leftover, this._Leftover.Length - count);
                
                this._ContentLengthCountdown -= count;
                
                return count;
            }
            
            int size = this._Leftover.Length;
            Array.Copy(this._Leftover, 0, buffer, offset, this._Leftover.Length);
            this._Leftover = Array.Empty<byte>();
            this._ContentLengthCountdown -= size;
            return size;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => this._ContentLength;
        public override long Position
        {
            get => this._ContentLength - this._ContentLengthCountdown;
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();
        
        public override void Flush() =>
            throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            throw new NotSupportedException();
    }
}
