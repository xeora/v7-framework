namespace Xeora.Web.Basics.Context
{
    public interface IHttpResponseStatus
    {
        short Code { get; set; }
        string Message { get; set; }
    }
}
