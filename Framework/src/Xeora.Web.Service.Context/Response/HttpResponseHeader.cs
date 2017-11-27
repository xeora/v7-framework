namespace Xeora.Web.Service.Context
{
    public class HttpResponseHeader : KeyValueCollection<string, string>, Basics.Context.IHttpResponseHeader
    {
        private Basics.Context.IHttpResponseStatus _Status;
        private Basics.Context.IHttpCookie _Cookie;

        public HttpResponseHeader()
        {
            this._Status = new HttpResponseStatus();
            this._Cookie = new HttpCookie();
        }

        public new void AddOrUpdate(string key, string value)
        {
            base.AddOrUpdate(key, value);
        }

        public Basics.Context.IHttpResponseStatus Status => this._Status;
        public Basics.Context.IHttpCookie Cookie => this._Cookie;
    }
}
