namespace Xeora.Web.Service.Session
{
    public interface IHttpSessionManager
    {
        void Acquire(string sessionId, out Basics.Session.IHttpSession sessionObject);
    }
}
