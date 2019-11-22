using System;

namespace Xeora.Web.Basics.Dss
{
    public interface IDss
    {
        string UniqueId { get; }
        bool Reusing { get; }
        DateTime Expires { get; }
        object this[string key] { get; set; }
        string[] Keys { get; }
    }
}
