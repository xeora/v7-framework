namespace Xeora.Web.Basics.Configuration
{
    public interface ISession
    {
        string CookieKey { get; }
        short Timeout { get; }
    }
}
