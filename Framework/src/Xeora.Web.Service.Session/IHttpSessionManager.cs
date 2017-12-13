using System.Net;

namespace Xeora.Web.Service.Session
{
    public interface IHttpSessionManager
    {
        void Acquire(IPAddress remoteIP, string sessionID, out Basics.Session.IHttpSession sessionObject);
    }
}
