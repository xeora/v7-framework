using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public delegate Basics.Context.IHttpCookieInfo SessionCookieRequestedHandler(bool skip);
    public class HttpResponse : Basics.Context.IHttpResponse
    {
        private string _ContextID;
        private string _TempLocation;
        private Stream _ResponseOutput;

        private string _RedirectedURL = string.Empty;

        public event SessionCookieRequestedHandler SessionCookieRequested;

        public HttpResponse(string contextID)
        {
            this._ContextID = contextID;
            this._TempLocation = 
                Path.Combine(
                    Configurations.Xeora.Application.Main.TemporaryRoot, 
                    string.Format("rs-{0}.bin", this._ContextID)
                );

            this._ResponseOutput = 
                new FileStream(this._TempLocation, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            this.Header = new HttpResponseHeader();
        }

        public Basics.Context.IHttpResponseHeader Header { get; private set; }

        private void PushHeaders(Net.NetworkStream streamEnclosure)
        {
            this.Header.AddOrUpdate("Date", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", new CultureInfo("en-US")));

            if (string.IsNullOrWhiteSpace(this.Header["Content-Type"]))
                this.Header.AddOrUpdate("Content-Type", "text/html");

            if (string.IsNullOrWhiteSpace(this.Header["Content-Length"]))
                this.Header.AddOrUpdate("Content-Length", this._ResponseOutput.Length.ToString());

            StringBuilder sB = new StringBuilder();

            sB.AppendFormat("HTTP/1.1 {0} {1}", this.Header.Status.Code, this.Header.Status.Message);
            sB.AppendLine();

            foreach (string key in this.Header.Keys)
            {
                sB.AppendFormat("{0}: {1}", key, this.Header[key]);
                sB.AppendLine();
            }

            string contentType = 
                this.Header["Content-Type"];
            bool skip = (string.IsNullOrEmpty(contentType) || contentType.IndexOf("text/html") == -1);
            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(skip));

            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this.Header.Cookie[key].ToString());
                sB.AppendLine();
            }

            sB.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            streamEnclosure.Write(buffer, 0, buffer.Length);
        }

        public void Write(string value, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(value);
            this.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count) =>
            this._ResponseOutput.Write(buffer, offset, count);

        public void Redirect(string URL) => this._RedirectedURL = URL;
        public bool IsRedirected => !string.IsNullOrEmpty(this._RedirectedURL);

        private void Redirect(Net.NetworkStream streamEnclosure)
        {
            StringBuilder sB = new StringBuilder();

            sB.AppendLine("HTTP/1.1 302 Object Moved");

            sB.AppendFormat("Date: {0}", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", new CultureInfo("en-US")));
            sB.AppendLine();

            sB.AppendFormat("Location: {0}", this._RedirectedURL);
            sB.AppendLine();

            sB.AppendLine("Connection: close");

            this.Header.Cookie.AddOrUpdate(SessionCookieRequested?.Invoke(false));

            // put cookies because it may contain sessionid
            foreach (string key in this.Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this.Header.Cookie[key].ToString());
                sB.AppendLine();
            }
            sB.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            streamEnclosure.Write(buffer, 0, buffer.Length);
        }

        public void Flush(Net.NetworkStream streamEnclosure)
        {
            if (this.IsRedirected)
            {
                this.Redirect(streamEnclosure);

                return;
            }

            this.PushHeaders(streamEnclosure);

            this._ResponseOutput.Seek(0, SeekOrigin.Begin);
            this._ResponseOutput.CopyTo(streamEnclosure);
        }

        internal void Dispose()
        {
            this._ResponseOutput.Close();

            File.Delete(this._TempLocation);
        }
    }
}
