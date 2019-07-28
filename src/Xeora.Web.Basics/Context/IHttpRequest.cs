using System.Net;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequest
    {
        IPAddress RemoteAddr { get; }
        Request.IHttpRequestHeader Header { get; }
        Request.IHttpRequestQueryString QueryString { get; }
        Request.IHttpRequestBody Body { get; }

        void RewritePath(string rawUrl);
    }
}
