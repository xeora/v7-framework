namespace Xeora.Web.Basics.Context
{
    public interface IHttpCookie : IKeyValueCollection<string, IHttpCookieInfo>
    {
        void AddOrUpdate(IHttpCookieInfo cookie);
        void Remove(string name);
        IHttpCookieInfo CreateNewCookie(string name);
    }
}
