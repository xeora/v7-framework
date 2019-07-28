namespace Xeora.Web.Basics.Context.Response
{
    public interface IHttpResponseHeader : IKeyValueCollection<string, string>
    {
        void AddOrUpdate(string key, string value);

        IHttpResponseStatus Status { get; }
        IHttpCookie Cookie { get; }
    }
}
