namespace Xeora.Web.Service.Session
{
    public interface IHttpSessionService
    {
        bool IsExpired { get; }
        void Extend();
    }
}
