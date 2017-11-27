namespace Xeora.Web.Basics.Context
{
    public interface IHttpCookie : IKeyValueCollection<string, IHttpCookieInfo>
    {
        void AddOrUpdate(IHttpCookieInfo cookie);
        IHttpCookieInfo CreateNewCookie(string name);
    }
}
