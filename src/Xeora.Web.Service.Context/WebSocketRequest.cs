using System.Net;
using Xeora.Web.Basics.Context.Request;
using Xeora.Web.Service.Context.Request;

namespace Xeora.Web.Service.Context
{
    public class WebSocketRequest : Basics.Context.IWebSocketRequest
    {
        public WebSocketRequest(IPAddress remoteAddr, IWebSocketRequestHeader header, IHttpRequestQueryString queryString)
        {
            this.RemoteAddr = remoteAddr;
            this.Header = header;
            this.QueryString = queryString;
        }

        public IPAddress RemoteAddr { get; }
        public IWebSocketRequestHeader Header { get; }
        public IHttpRequestQueryString QueryString { get; private set; }
        
        public void RewritePath(string rawUrl)
        {
            ((WebSocketRequestHeader)this.Header).ReplacePath(rawUrl);
            this.QueryString = new HttpRequestQueryString(this.Header.Url);
        }
    }
}
