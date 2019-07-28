using System;
using System.IO;
using System.Text;
using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public class HttpRequestFileInfo : Basics.Context.IHttpRequestFileInfo
    {
        internal Stream _ContentStream;

        private readonly string _TempLocation;

        public HttpRequestFileInfo(string contextId, string contentType, Encoding contentEncoding, string fileName, bool keepInMemory)
        {
            this._TempLocation = 
                Path.Combine(Configurations.Xeora.Application.Main.TemporaryRoot, string.Format("fs-{0}.bin", contextId));

            if (keepInMemory)
                this._ContentStream = new MemoryStream();
            else
                this._ContentStream = 
                    new FileStream(this._TempLocation, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.FileName = fileName;
        }

        public string ContentType { get; private set; }
        public Encoding ContentEncoding { get; private set; }
        public string FileName { get; private set; }
        public long Length => this._ContentStream.Length;
        public Stream Stream => this._ContentStream;

        internal void Dispose()
        {
            this._ContentStream.Close();

            if (File.Exists(this._TempLocation))
                File.Delete(this._TempLocation);
        }
    }
}
