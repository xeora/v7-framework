using System.IO;
using System.Net;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        private Stream _RequestInput;

        private IPAddress _RemoteAddr;
        private Basics.Context.IHttpRequestHeader _Header;
        private Basics.Context.IHttpRequestQueryString _QueryString;
        private Basics.Context.IHttpRequestBody _Body;

        public HttpRequest(IPAddress remoteAddr, ref Stream requestInput)
        {
            this._RemoteAddr = remoteAddr;
            this._RequestInput = requestInput;
        }

        public IPAddress RemoteAddr => this._RemoteAddr;
        public Basics.Context.IHttpRequestHeader Header => this._Header;
        public Basics.Context.IHttpRequestQueryString QueryString => this._QueryString;
        public Basics.Context.IHttpRequestBody Body => this._Body;

        private long _HeaderEndIndex = -1;
        public bool Build(string contextID)
        {
            if (this._HeaderEndIndex == -1)
            {
                this._Header = new HttpRequestHeader(ref this._RequestInput);

                if (!((HttpRequestHeader)this._Header).EOF)
                    return false;

                this._HeaderEndIndex = this._RequestInput.Position;
                this._QueryString = new HttpRequestQueryString(this._Header.URL);
            }

            if (this._Header.ContentLength > 0)
            {
                if (this._Header.ContentLength != (this._RequestInput.Length - this._HeaderEndIndex))
                    return false;
            }

            this._RequestInput.Seek(this._HeaderEndIndex, SeekOrigin.Begin);
            this._Body = new HttpRequestBody(contextID, this._Header, ref this._RequestInput);

            return true;
        }

        public void RewritePath(string rawURL)
        {
            ((HttpRequestHeader)this._Header).ReplacePath(rawURL);
            this._QueryString = new HttpRequestQueryString(this._Header.URL);
        }

        internal void Dispose() =>
            ((HttpRequestBody)this._Body).Dispose();
    }
}
