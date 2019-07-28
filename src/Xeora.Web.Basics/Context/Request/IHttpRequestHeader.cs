using System.Text;

namespace Xeora.Web.Basics.Context.Request
{
    public interface IHttpRequestHeader : IKeyValueCollection<string, string>
    {
        HttpMethod Method { get; }
        IUrl Url { get; }
        string Protocol { get; }

        string Host { get; }
        string UserAgent { get; }
        int ContentLength { get; }
        string ContentType { get; }
        Encoding ContentEncoding { get; }
        string Boundary { get; }

        IHttpCookie Cookie { get; }
    }
}
