using System;

namespace Xeora.Web.Basics.DSS
{
    public interface IDSS
    {
        string UniqueID { get; }
        DateTime Expires { get; }
        object this[string key] { get; set; }
        string[] Keys { get; }
    }
}
