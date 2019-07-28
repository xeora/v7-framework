using System;
using Xeora.Web.Basics.Application;
using Xeora.Web.Basics.Session;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpContext : IKeyValueCollection<string, object>, IDisposable
    {
        string UniqueId { get; }
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        IHttpSession Session { get; }
        IHttpApplication Application { get; }

        void AddOrUpdate(string key, object value);

        string HashCode { get; }
    }
}
