namespace Xeora.Web.Service.Context
{
    public class HttpCookie : KeyValueCollection<string, Basics.Context.IHttpCookieInfo>, Basics.Context.IHttpCookie
    {
        public void AddOrUpdate(Basics.Context.IHttpCookieInfo cookie)
        {
            if (cookie != null)
                base.AddOrUpdate(cookie.Name, cookie);
        }

        public new void Remove(string name)
        {
            if (ContainsKey(name)) 
                base.Remove(name);
        }

        public Basics.Context.IHttpCookieInfo CreateNewCookie(string name) =>
            new HttpCookieInfo(name);
    }
}
