namespace Xeora.Web.External.Service.Session
{
    public interface IHttpSessionService
    {
        bool IsExpired { get; }
        void Extend();
    }
}
