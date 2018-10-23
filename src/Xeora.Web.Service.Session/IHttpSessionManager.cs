using System.Net;

namespace Xeora.Web.Service.Session
{
    public interface IHttpSessionManager
    {
        void Acquire(string sessionID, out Basics.Session.IHttpSession sessionObject);
    }
}
