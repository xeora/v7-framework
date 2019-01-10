namespace Xeora.Web.Service.Context
{
    public class HttpResponseHeader : KeyValueCollection<string, string>, Basics.Context.IHttpResponseHeader
    {
        public HttpResponseHeader()
        {
            this.Status = new HttpResponseStatus();
            this.Cookie = new HttpCookie();
        }

        public new void AddOrUpdate(string key, string value)
        {
            base.AddOrUpdate(key, value);
        }

        public Basics.Context.IHttpResponseStatus Status { get; private set; };
        public Basics.Context.IHttpCookie Cookie { get; private set; };
    }
}
