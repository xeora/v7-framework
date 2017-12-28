using System.Text;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpResponse
    {
        IHttpResponseHeader Header { get; }

        void Write(string value, Encoding encoding);
        void Write(byte[] buffer, int offset, int count);

        void Redirect(string URL);
        bool IsRedirected { get; }
    }
}
