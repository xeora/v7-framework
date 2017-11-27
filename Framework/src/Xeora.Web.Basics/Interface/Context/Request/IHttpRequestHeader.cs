using System.Text;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequestHeader : IKeyValueCollection<string, string>
    {
        HttpMethod Method { get; }
        IURL URL { get; }
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
