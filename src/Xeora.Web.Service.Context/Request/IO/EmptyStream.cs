using System;
using System.IO;

namespace Xeora.Web.Service.Context.Request.IO
{
    public class EmptyStream : BodyStream
    {
        public override int Read(byte[] buffer, int offset, int count) => 0;
        internal override ParserResultTypes ReadAllInto(ref Stream contentStream) => ParserResultTypes.Success;
        public override void Conclude() {}

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position
        {
            get => 0;
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
