namespace Xeora.Web.Basics.Context.Request
{
    public interface IWebSocketRequestHeader : IKeyValueCollection<string, string>
    {
        IUrl Url { get; }
        string Protocol { get; }

        string Host { get; }
        string UserAgent { get; }
        
        string Key { get; set; }
        string Extensions { get; set; }
        string Version { get; set; }

        IHttpCookie Cookie { get; }
    }
}
