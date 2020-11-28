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
    public class HttpResponse : Basics.Context.IHttpResponse
    {
        public static readonly char[] Newline = { '\r', '\n' };

        private readonly string _TempLocation;
        private readonly Stream _ResponseOutput;
        private readonly bool _KeepAlive;
        private readonly Action<Basics.Context.Response.IHttpResponseHeader> _ServerResponseStampHandler;

        private bool _HeaderFlushed;
        private string _RedirectedUrl = string.Empty;

        public event SessionCookieRequestedHandler SessionCookieRequested;
        public event StreamEnclosureRequestedHandler StreamEnclosureRequested;

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
            
            this.Header.AddOrUpdate("Date", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(this.Header["Content-Type"]))
                this.Header.AddOrUpdate("Content-Type", "text/html");

            if (string.IsNullOrWhiteSpace(this.Header["Content-Length"]))
                this.Header.AddOrUpdate("Content-Length", this._ResponseOutput.Length.ToString());

            this.Header.AddOrUpdate("Connection", this._KeepAlive ? "keep-alive" : "close");
            if (this._KeepAlive)
                this.Header.AddOrUpdate("Keep-Alive", $"timeout={streamEnclosure.ReadTimeout / 1000}");

            this._ServerResponseStampHandler(this.Header);
            
            StringBuilder sB = new StringBuilder();

            sB.AppendFormat("HTTP/1.1 {0} {1}", this.Header.Status.Code, this.Header.Status.Message);
            sB.Append(Newline);

            foreach (string key in this.Header.Keys)
            {
                sB.AppendFormat("{0}: {1}", key, this.Header[key]);
                sB.Append(Newline);
            }

            string contentType = 
                this.Header["Content-Type"];
            bool skip = string.IsNullOrEmpty(contentType) || contentType.IndexOf("text/html", StringComparison.Ordinal) == -1;
            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(skip));

            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this.Header.Cookie[key]);
                sB.Append(Newline);
            }

            sB.Append(Newline);

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            streamEnclosure.Write(buffer, 0, buffer.Length);

            this._HeaderFlushed = true;
        }

        public void ActivateStreaming()
        {
            if (this.IsRedirected)
                throw new Exception("Not possible to activate streaming when request has been already redirected!");
            
            if (string.IsNullOrWhiteSpace(this.Header["Content-Length"]))
                throw new Exception("Content-Length should be defined in header to activate streaming for http response!");

            Net.NetworkStream streamEnclosure = null;
            StreamEnclosureRequested?.Invoke(out streamEnclosure);
            
            if (streamEnclosure == null)
                throw new Exception("Inaccessible stream enclosure to activate streaming on http response!");
            
            this.PushHeaders(streamEnclosure);
        }
        
        public void Write(string value, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(value);
            this.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count) =>
            this._ResponseOutput.Write(buffer, offset, count);

        public void Redirect(string url) => this._RedirectedUrl = url;
        public bool IsRedirected => !string.IsNullOrEmpty(this._RedirectedUrl);

        private void Redirect(Stream streamEnclosure)
        {
            StringBuilder sB = new StringBuilder();

            sB.Append("HTTP/1.1 302 Object Moved");
            sB.Append(Newline);

            sB.AppendFormat("Date: {0}", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture));
            sB.Append(Newline);

            sB.AppendFormat("Location: {0}", this._RedirectedUrl);
            sB.Append(Newline);

            sB.Append("Connection: close");
            sB.Append(Newline);

            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(false));

            // put cookies because it may contain sessionId
            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this.Header.Cookie[key]);
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
