using System;

namespace Xeora.Web.Basics.Session
{
    public interface IHttpSession
    {
        string SessionId { get; }
        DateTime Expires { get; }
        object this[string key] { get; set; }
        string[] Keys { get; }
        object Lock(string key, Func<string, object, object> lockHandler);
    }
}
