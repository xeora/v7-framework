using System.Net;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequest
    {
        IPAddress RemoteAddr { get; }
        IHttpRequestHeader Header { get; }
        IHttpRequestQueryString QueryString { get; }
        IHttpRequestBody Body { get; }

        void RewritePath(string rawURL);
    }
}
