using System;

namespace Xeora.Web.Basics.Dss
{
    public interface IDss
    {
        string UniqueId { get; }
        bool Reusing { get; }
        DateTime Expires { get; }
        string[] Keys { get; }
        object Get(string key, string lockCode = null);
        void Set(string key, object value, string lockCode = null);
        string Lock(string key);
        void Release(string key, string lockCode);
    }
}
