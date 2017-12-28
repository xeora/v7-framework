using System.IO;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequestBody
    {
        IHttpRequestForm Form { get; }
        IHttpRequestFile File { get; }
        Stream ContentStream { get; }
    }
}
