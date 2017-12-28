using System.Net;

namespace Xeora.Web.Basics.Configuration
{
    public interface ISession
    {
        SessionServiceTypes ServiceType { get; }
        IPEndPoint ServiceEndPoint { get; }
        string CookieKey { get; }
        short Timeout { get; }
    }
}
