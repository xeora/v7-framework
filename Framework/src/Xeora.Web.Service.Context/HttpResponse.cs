using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public delegate Basics.Context.IHttpCookieInfo SessionCookieRequestedHandler();
    public class HttpResponse : Basics.Context.IHttpResponse
    {
        private string _ContextID;
        private string _TempLocation;
        private Stream _ResponseOutput;

        private Basics.Context.IHttpResponseHeader _Header;
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

            this._Header = new HttpResponseHeader();
        }

        public Basics.Context.IHttpResponseHeader Header => this._Header;

        private void PushHeaders(ref NetworkStream remoteStream)
        {
            this._Header.AddOrUpdate("Date", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", new CultureInfo("en-US")));

            if (string.IsNullOrWhiteSpace(this._Header["Content-Type"]))
                this._Header.AddOrUpdate("Content-Type", "text/html");

            if (string.IsNullOrWhiteSpace(this._Header["Content-Length"]))
                this._Header.AddOrUpdate("Content-Length", this._ResponseOutput.Length.ToString());

            StringBuilder sB = new StringBuilder();

            sB.AppendFormat("HTTP/1.1 {0} {1}", this._Header.Status.Code, this._Header.Status.Message);
            sB.AppendLine();

            foreach (string key in this._Header.Keys)
            {
                sB.AppendFormat("{0}: {1}", key, this._Header[key]);
                sB.AppendLine();
            }

            this._Header.Cookie.AddOrUpdate(SessionCookieRequested());

            foreach (string key in this._Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this._Header.Cookie[key].ToString());
                sB.AppendLine();
            }

            sB.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            remoteStream.Write(buffer, 0, buffer.Length);
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

        private void Redirect(ref NetworkStream remoteStream)
        {
            StringBuilder sB = new StringBuilder();

            sB.AppendLine("HTTP/1.1 302 Object Moved");

            sB.AppendFormat("Date: {0}", DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", new CultureInfo("en-US")));
            sB.AppendLine();

            sB.AppendFormat("Location: {0}", this._RedirectedURL);
            sB.AppendLine();

            sB.AppendLine("Connection: close");

            this._Header.Cookie.AddOrUpdate(SessionCookieRequested());

            // put cookies because it may contain sessionid
            foreach (string key in this._Header.Cookie.Keys)
            {
                sB.AppendFormat("Set-Cookie: {0}", this._Header.Cookie[key].ToString());
                sB.AppendLine();
            }
            sB.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
            remoteStream.Write(buffer, 0, buffer.Length);
        }

        public void Flush(ref NetworkStream remoteStream)
        {
            if (this.IsRedirected)
            {
                this.Redirect(ref remoteStream);

                return;
            }

            this.PushHeaders(ref remoteStream);

            this._ResponseOutput.Seek(0, SeekOrigin.Begin);
            this._ResponseOutput.CopyTo(remoteStream);
        }

        internal void Dispose()
        {
            this._ResponseOutput.Close();

            File.Delete(this._TempLocation);
        }
    }
}
