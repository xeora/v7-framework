namespace Xeora.Web.Basics.Context
{
    public interface IHttpRequestBody
    {
        IHttpRequestForm Form { get; }
        IHttpRequestFile File { get; }
    }
}
