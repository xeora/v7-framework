namespace Xeora.Web.Basics.Context.Response
{
    public interface IHttpResponseStatus
    {
        short Code { get; set; }
        string Message { get; }
    }
}
