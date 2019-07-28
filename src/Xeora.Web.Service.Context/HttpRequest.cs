using System.Net;
using Xeora.Web.Service.Context.Request;
using HttpRequestHeader = Xeora.Web.Service.Context.Request.HttpRequestHeader;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        private HttpRequestHeader _Header;

        public HttpRequest(IPAddress remoteAddr) =>
            this.RemoteAddr = remoteAddr;

        public IPAddress RemoteAddr { get; }
        public Basics.Context.Request.IHttpRequestHeader Header => this._Header;
        public Basics.Context.Request.IHttpRequestQueryString QueryString { get; private set; }
        public Basics.Context.Request.IHttpRequestBody Body { get; private set; }

        public bool Build(string contextId, Net.NetworkStream streamEnclosure)
        {
            this._Header = new HttpRequestHeader(streamEnclosure);
            if (!this._Header.Parse())
                return false;

            this.QueryString = new HttpRequestQueryString(this._Header.Url);
            this.Body = new HttpRequestBody(contextId, this._Header, streamEnclosure);

            return true;
        }

        public void RewritePath(string rawUrl)
        {
            ((HttpRequestHeader)this.Header).ReplacePath(rawUrl);
            this.QueryString = new HttpRequestQueryString(this.Header.Url);
        }

        internal void Dispose() =>
            ((HttpRequestBody)this.Body).Dispose();
    }
}
