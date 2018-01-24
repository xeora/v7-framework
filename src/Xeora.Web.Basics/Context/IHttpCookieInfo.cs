using System;

namespace Xeora.Web.Basics.Context
{
    public interface IHttpCookieInfo
    {
        string Name { get; }
        string Value { get; set; }
        DateTime Expires { get; set; }
        string Domain { get; set; }
        string Path { get; set; }
        bool Secure { get; set; }
        bool HttpOnly { get; set; }
    }
}
