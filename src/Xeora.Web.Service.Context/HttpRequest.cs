using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        public HttpRequest(IPAddress remoteAddr) =>
            this.RemoteAddr = remoteAddr;

        public IPAddress RemoteAddr { get; private set; }
        public Basics.Context.IHttpRequestHeader Header { get; private set; }
        public Basics.Context.IHttpRequestQueryString QueryString { get; private set; }
        public Basics.Context.IHttpRequestBody Body { get; private set; }

        public void Build(string contextID, ref NetworkStream remoteStream)
        {
            this.Header = new HttpRequestHeader(ref remoteStream);
            this.QueryString = new HttpRequestQueryString(this.Header.URL);
            this.Body = 
                new HttpRequestBody(
                    contextID, 
                    this.Header, 
                    ((HttpRequestHeader)this.Header).Residual, 
                    ref remoteStream
                );
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
