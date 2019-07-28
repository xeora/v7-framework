namespace Xeora.Web.Service.Context.Response
{
    public class HttpResponseHeader : KeyValueCollection<string, string>, Basics.Context.Response.IHttpResponseHeader
    {
        public HttpResponseHeader()
        {
            this.Status = new HttpResponseStatus();
            this.Cookie = new HttpCookie();
        }

        public new void AddOrUpdate(string key, string value) =>
            base.AddOrUpdate(key, value);

        public Basics.Context.Response.IHttpResponseStatus Status { get; }
        public Basics.Context.IHttpCookie Cookie { get; }
    }
}
