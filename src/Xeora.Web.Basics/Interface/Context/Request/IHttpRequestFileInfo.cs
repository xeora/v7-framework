using System.IO;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequestFileInfo
    {
        string ContentType { get; }
        string FileName { get; }
        long Length { get; }
        Stream Stream { get; }
    }
}
