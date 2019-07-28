using System.IO;

namespace Xeora.Web.Basics.Context.Request
{
    public interface IHttpRequestBody
    {
        IHttpRequestForm Form { get; }
        IHttpRequestFile File { get; }
        Stream ContentStream { get; }
    }
}
