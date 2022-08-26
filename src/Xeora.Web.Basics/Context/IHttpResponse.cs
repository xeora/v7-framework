using System.Text;
using Xeora.Web.Basics.Context.Response;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpResponse
    {
        Response.IHttpResponseHeader Header { get; }

        IHttpResponseStream OpenStreaming();
        void Write(string value, Encoding encoding);
        void Write(byte[] buffer, int offset, int count);

        void Redirect(string url);
        bool IsRedirected { get; }
    }
}
