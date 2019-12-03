using System.IO;
using System.Text;
using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestFileInfo : Basics.Context.Request.IHttpRequestFileInfo
    {
        internal readonly Stream ContentStream;
        private readonly string _TempLocation;

        public HttpRequestFileInfo(string contextId, string contentType, Encoding contentEncoding, string fileName, bool keepInMemory)
        {
            this._TempLocation = 
                Path.Combine(Configurations.Xeora.Application.Main.TemporaryRoot, $"fs-{contextId}.bin");

            if (keepInMemory)
                this.ContentStream = new MemoryStream();
            else
                this.ContentStream = 
                    new FileStream(this._TempLocation, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.FileName = fileName;
        }

        public string ContentType { get; }
        public Encoding ContentEncoding { get; }
        public string FileName { get; }
        public long Length => this.ContentStream.Length;
        public Stream Stream => this.ContentStream;

        internal void Dispose()
        {
            this.ContentStream.Close();

            if (File.Exists(this._TempLocation))
                File.Delete(this._TempLocation);
        }
    }
}
