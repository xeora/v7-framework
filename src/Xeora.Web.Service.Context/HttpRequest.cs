using System.Net;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        private HttpRequestHeader _Header;

        public HttpRequest(IPAddress remoteAddr) =>
            this.RemoteAddr = remoteAddr;

        public IPAddress RemoteAddr { get; private set; }
        public Basics.Context.IHttpRequestHeader Header => this._Header;
        public Basics.Context.IHttpRequestQueryString QueryString { get; private set; }
        public Basics.Context.IHttpRequestBody Body { get; private set; }

        public bool Build(string contextId, Net.NetworkStream streamEnclosure)
        {
            this._Header = new HttpRequestHeader(streamEnclosure);
            if (!this._Header.Parse())
                return false;

            this.QueryString = new HttpRequestQueryString(this._Header.URL);
            this.Body = new HttpRequestBody(contextId, this._Header, streamEnclosure);

            return true;
        }

        public void RewritePath(string rawURL)
        {
            ((HttpRequestHeader)this.Header).ReplacePath(rawURL);
            this.QueryString = new HttpRequestQueryString(this.Header.URL);
        }

        internal void Dispose() =>
            ((HttpRequestBody)this.Body).Dispose();
    }
}
