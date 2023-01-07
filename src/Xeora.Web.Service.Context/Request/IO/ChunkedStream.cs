using System;
using System.IO;

namespace Xeora.Web.Service.Context.Request.IO
{
    public class ChunkedStream : BodyStream
    {
        private byte[] _ChunkLeftover = Array.Empty<byte>();
        private long _Position;
        private bool _ChunkCompleted;
        private bool _Concluded;
        
        public ChunkedStream(Net.NetworkStream streamEnclosure) :
            base(streamEnclosure) {}

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this._ChunkCompleted && this._ChunkLeftover.Length == 0) return 0;
                
            ParserResultTypes parserResult = 
                this.ReadChunked(buffer, offset, count, out int read);
            if (parserResult != ParserResultTypes.Success)
                throw new IOException("Malformed request body");
            return read;
        }

        internal override ParserResultTypes ReadAllInto(ref Stream contentStream)
        {
            contentStream ??= new MemoryStream();
            
            byte[] buffer = new byte[BUFFER_SIZE];
            
            do
            {
                if (this._ChunkCompleted && this._ChunkLeftover.Length == 0) 
                    return ParserResultTypes.Success;
                
                ParserResultTypes parserResult =
                    this.ReadChunked(buffer, 0, buffer.Length, out int read);
                if (parserResult != ParserResultTypes.Success) return parserResult;
                if (read == 0) return ParserResultTypes.Success;
                    
                contentStream.Write(buffer, 0, read);
            } while (true);
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

                this._ChunkLeftover =
                    ((MemoryStream)cacheContent).GetBuffer();
            }
            finally
            {
                cacheContent?.Dispose();
            }
        }

        private ParserResultTypes ReadChunked(byte[] buffer, int offset, int count, out int read)
        {
            read = this.ServeLeftOver(buffer, offset, count);
            if (read > 0) return ParserResultTypes.Success;

            Stream bufferStream = null;

            const string rn = "\r\n";
            const int nl = 2;

            string content = string.Empty;

            byte[] chunkBuffer = 
                new byte[sbyte.MaxValue];

            try
            {
                bufferStream = new MemoryStream();

                do
                {
                    int bR = this.StreamEnclosure.Read(chunkBuffer, 0, chunkBuffer.Length);

                    if (bR == 0)
                    {
                        if (this.StreamEnclosure.Disposed)
                            return ParserResultTypes.BadRequest;
                        
                        System.Threading.Thread.Sleep(1);

                        continue;
                    }

                    content += System.Text.Encoding.ASCII.GetString(chunkBuffer, 0, bR);

                    int eofIndex = content.IndexOf(rn, StringComparison.Ordinal);
                    if (eofIndex == -1) continue;

                    // Clean up unrelated characters
                    content = content.Remove(eofIndex);
                    
                    // Check if there is any chunk-extension
                    // https://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.6.1
                    int separatorIndex = content.IndexOf(';', 0);
                    if (separatorIndex == -1)
                    {
                        separatorIndex = content.IndexOf(' ', 0);
                        if (separatorIndex == -1) separatorIndex = eofIndex;
                    }

                    int contentLength;
                    try
                    {
                        contentLength = 
                            Convert.ToInt32(content[..separatorIndex], 16);
                    }
                    catch
                    {
                        return ParserResultTypes.BadRequest;
                    }
                    
                    eofIndex += nl;
                    this.StreamEnclosure.Return(chunkBuffer, eofIndex, bR - eofIndex);
                    
                    if (contentLength == 0)
                    {
                        this.StreamEnclosure.Throw(nl);
                        this._ChunkCompleted = true;
                        break;
                    }
                    
                    byte[] chunkContentBytes = 
                        new byte[contentLength + nl];
                    bR = this.ReadWithLength(chunkContentBytes, 0, chunkContentBytes.Length);
                    if (bR != chunkContentBytes.Length) return ParserResultTypes.BadRequest;
                    
                    bufferStream.Write(chunkContentBytes, 0, bR - nl);
                    if (bufferStream.Length >= count) break;
                    
                    content = string.Empty;
                } while (true);
                
                read = this.ReadInto(ref bufferStream, buffer, offset, count);
                this._Position += read;
                
                return ParserResultTypes.Success;
            }
            finally
            {
                bufferStream?.Dispose();
            }
        }

        private int ReadInto(ref Stream stream, byte[] buffer, int offset, int count)
        {
            stream.Seek(0, SeekOrigin.Begin);
            int bR = stream.Read(buffer, offset, count);

            long leftOverSize = stream.Length - stream.Position;
            if (leftOverSize == 0) return bR;
            
            byte[] leftOverBytes = new byte[leftOverSize];
            int read = stream.Read(leftOverBytes, 0, leftOverBytes.Length);
            this.Return(leftOverBytes, 0, read);

            return bR;
        }

        private void Return(byte[] buffer, int offset, int count)
        {
            byte[] newLeftOver = new byte[count + this._ChunkLeftover.Length];
            
            Array.Copy(buffer, offset, newLeftOver, 0, count);
            Array.Copy(this._ChunkLeftover, 0, newLeftOver, count, this._ChunkLeftover.Length);

            this._ChunkLeftover = newLeftOver;
        }
        
        private int ServeLeftOver(byte[] buffer, int offset, int count)
        {
            if (this._ChunkLeftover.Length == 0) return 0;

            if (this._ChunkLeftover.Length > count)
            {
                Array.Copy(this._ChunkLeftover, 0, buffer, offset, count);
                Array.Copy(this._ChunkLeftover, count, this._ChunkLeftover, 0, this._ChunkLeftover.Length - count);
                Array.Resize(ref this._ChunkLeftover, this._ChunkLeftover.Length - count);

                this._Position += count;
                
                return count;
            }
            
            int size = this._ChunkLeftover.Length;
            Array.Copy(this._ChunkLeftover, 0, buffer, offset, this._ChunkLeftover.Length);
            this._ChunkLeftover = Array.Empty<byte>();
            this._Position += size;
            return size;
        }
        
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => -1;
        public override long Position
        {
            get => this._Position;
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
