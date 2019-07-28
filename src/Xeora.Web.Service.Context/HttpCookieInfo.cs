using System;
using System.Globalization;

namespace Xeora.Web.Service.Context
{
    public class HttpCookieInfo : Basics.Context.IHttpCookieInfo
    {
        public HttpCookieInfo(string name) =>
            this.Name = name;

        public string Name { get; }
        public string Value { get; set; }
        public DateTime Expires { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }

        public override string ToString()
        {
            string rCookie =
                $"{this.Name}={System.Web.HttpUtility.UrlEncode(this.Value)}";

            if (!this.Expires.Equals(DateTime.MinValue))
            {
                string expires =
                    this.Expires.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
                rCookie = $"{rCookie}; expires={expires}";
            }

            if (!string.IsNullOrEmpty(this.Domain))
                rCookie = $"{rCookie}; domain={this.Domain}";

            if (string.IsNullOrEmpty(this.Path))
                this.Path = "/";
            rCookie = $"{rCookie}; path={this.Path}";

            if (this.Secure)
                rCookie = $"{rCookie}; Secure";

            if (this.HttpOnly)
                rCookie = $"{rCookie}; HttpOnly";

            return rCookie;
        }
    }
}
