using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        private IPAddress _RemoteAddr;
        private Basics.Context.IHttpRequestHeader _Header;
        private Basics.Context.IHttpRequestQueryString _QueryString;
        private Basics.Context.IHttpRequestBody _Body;

        public HttpRequest(IPAddress remoteAddr) =>
            this._RemoteAddr = remoteAddr;

        public IPAddress RemoteAddr => this._RemoteAddr;
        public Basics.Context.IHttpRequestHeader Header => this._Header;
        public Basics.Context.IHttpRequestQueryString QueryString => this._QueryString;
        public Basics.Context.IHttpRequestBody Body => this._Body;

        public void Build(string contextID, ref NetworkStream remoteStream)
        {
            this._Header = new HttpRequestHeader(ref remoteStream);
            this._QueryString = new HttpRequestQueryString(this._Header.URL);
            this._Body = 
                new HttpRequestBody(
                    contextID, 
                    this._Header, 
                    ((HttpRequestHeader)this._Header).Residual, 
                    ref remoteStream
                );
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
