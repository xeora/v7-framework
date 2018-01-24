namespace Xeora.Web.Basics.Application
{
    public interface IHttpApplication
    {
        object this[string key] { get; set; }
        string[] Keys { get; }
    }
}
