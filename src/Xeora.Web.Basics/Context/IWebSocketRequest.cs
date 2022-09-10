using System.Net;

namespace Xeora.Web.Basics.Context
{
    public interface IWebSocketRequest
    {
        IPAddress RemoteAddr { get; }
        Request.IWebSocketRequestHeader Header { get; }
        Request.IHttpRequestQueryString QueryString { get; }
        
        void RewritePath(string rawUrl);
    }
}
