using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Context.Response;

namespace Xeora.Web.Service.Context
{
    public delegate Basics.Context.IHttpCookieInfo SessionCookieRequestedHandler(bool skip);
    public delegate void StreamEnclosureRequestedHandler(out Net.NetworkStream streamEnclosure);
    public delegate void ConcludeRequestRequestedHandler();
    public class HttpResponse : Basics.Context.IHttpResponse
    {
        private const string HTTP_VERSION_1_1 = "HTTP/1.1";
        
        private const string HEADER_DATE = "Date";
        private const string HEADER_CONTENT_TYPE = "Content-Type";
        private const string HEADER_CONTENT_LENGTH = "Content-Length";
        private const string HEADER_TRANSFER_ENCODING = "Transfer-Encoding";
        private const string HEADER_LOCATION = "Location";
        private const string HEADER_KEEP_ALIVE = "Keep-Alive";
        private const string HEADER_SET_COOKIE = "Set-Cookie";
        private const string HEADER_CONNECTION = "Connection";
        
        public static readonly char[] Newline = { '\r', '\n' };

        private readonly string _TempLocation;
        private readonly Stream _ResponseOutput;
        private readonly bool _KeepAlive;
        private readonly Action<Basics.Context.Response.IHttpResponseHeader> _ServerResponseStampHandler;

        private bool _HeaderFlushed;
        private bool _Chunked;
        private string _RedirectedUrl = string.Empty;

        public event SessionCookieRequestedHandler SessionCookieRequested;
        public event StreamEnclosureRequestedHandler StreamEnclosureRequested;
        public event ConcludeRequestRequestedHandler ConcludeRequestRequested;

        public HttpResponse(string contextId, bool keepAlive, 
            Action<Basics.Context.Response.IHttpResponseHeader> serverResponseStampHandler)
        {
            this._TempLocation = 
                Path.Combine(
                    Configurations.Xeora.Application.Main.TemporaryRoot,
                    $"rs-{contextId}.bin"
                );

            this._ResponseOutput = 
                new FileStream(this._TempLocation, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            this._KeepAlive = keepAlive;
            this._ServerResponseStampHandler = serverResponseStampHandler;

            this.Header = new HttpResponseHeader();
        }
        
        public Basics.Context.Response.IHttpResponseHeader Header { get; }

        private void PushHeaders(Stream streamEnclosure)
        {
            if (this._HeaderFlushed) return;
            
            ConcludeRequestRequested?.Invoke();
            
            this.Header.AddOrUpdate(HEADER_DATE, DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(this.Header[HEADER_CONTENT_TYPE]))
                this.Header.AddOrUpdate(HEADER_CONTENT_TYPE, "text/html");

            if (this._Chunked)
            {
                ((HttpResponseHeader) this.Header).Remove(HEADER_CONTENT_LENGTH);
                this.Header.AddOrUpdate(HEADER_TRANSFER_ENCODING, "chunked");
            }
            else if (string.IsNullOrWhiteSpace(this.Header[HEADER_CONTENT_LENGTH]))
                this.Header.AddOrUpdate(HEADER_CONTENT_LENGTH, this._ResponseOutput.Length.ToString());

            this.Header.AddOrUpdate(HEADER_CONNECTION, this._KeepAlive ? "keep-alive" : "close");
            if (this._KeepAlive)
                this.Header.AddOrUpdate(HEADER_KEEP_ALIVE, $"timeout={streamEnclosure.ReadTimeout / 1000}");

            this._ServerResponseStampHandler(this.Header);
            
            StringBuilder sB = new StringBuilder();

            sB.Append($"{HTTP_VERSION_1_1} {this.Header.Status.Code} {this.Header.Status.Message}");
            sB.Append(Newline);

            foreach (string key in this.Header.Keys)
            {
                sB.AppendFormat("{0}: {1}", key, this.Header[key]);
                sB.Append(Newline);
            }

            string contentType = 
                this.Header[HEADER_CONTENT_TYPE];
            bool skip = string.IsNullOrEmpty(contentType) || contentType.IndexOf("text/html", StringComparison.Ordinal) == -1;
            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(skip));

            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.Append($"{HEADER_SET_COOKIE}: {this.Header.Cookie[key]}");
                sB.Append(Newline);
            }

            sB.Append(Newline);

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            streamEnclosure.Write(buffer, 0, buffer.Length);

            this._HeaderFlushed = true;
        }

        public Basics.Context.Response.IHttpResponseStream OpenStreaming(long contentLength = 0)
        {
            if (this.IsRedirected)
                throw new Exception("Not possible to activate streaming when request has been already redirected!");

            Net.NetworkStream streamEnclosure = null;
            StreamEnclosureRequested?.Invoke(out streamEnclosure);
            
            if (streamEnclosure == null)
                throw new Exception("Inaccessible stream enclosure to activate streaming on http response!");
            
            this._Chunked = contentLength == 0;
            if (!this._Chunked) this.Header.AddOrUpdate(HEADER_CONTENT_LENGTH, contentLength.ToString());
            this.PushHeaders(streamEnclosure);

            return new HttpResponseStream(streamEnclosure, this._Chunked);
        }
        
        public void Write(string value, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(value);
            this.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (this._Chunked)
                throw new Exception("It is not allowed to use response write while streaming is activated!");
            
            this._ResponseOutput.Write(buffer, offset, count);
        }

        public void Redirect(string url) => this._RedirectedUrl = url;
        public bool IsRedirected => !string.IsNullOrEmpty(this._RedirectedUrl);

        private void Redirect(Stream streamEnclosure)
        {
            StringBuilder sB = new StringBuilder();

            sB.Append($"{HTTP_VERSION_1_1} 302 Object Moved");
            sB.Append(Newline);

            sB.Append($"{HEADER_DATE}: {DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture)}");
            sB.Append(Newline);

            sB.Append($"{HEADER_LOCATION}: {this._RedirectedUrl}");
            sB.Append(Newline);

            sB.Append($"{HEADER_CONNECTION}: close");
            sB.Append(Newline);

            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(false));

            // put cookies because it may contain sessionId
            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.Append($"{HEADER_SET_COOKIE}: {this.Header.Cookie[key]}");
                sB.Append(Newline);
            }
            sB.Append(Newline);

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            streamEnclosure.Write(buffer, 0, buffer.Length);
        }

        public void Flush(Net.NetworkStream streamEnclosure)
        {
            if (this.IsRedirected)
            {
                this.Redirect(streamEnclosure);
                
                streamEnclosure.KeepAlive = false;

                return;
            }

            this.PushHeaders(streamEnclosure);
            
            this._ResponseOutput.Seek(0, SeekOrigin.Begin);
            this._ResponseOutput.CopyTo(streamEnclosure);
            
            streamEnclosure.KeepAlive = this._KeepAlive;
        }

        internal void Dispose()
        {
            this._ResponseOutput.Close();

            File.Delete(this._TempLocation);
        }
    }
}
