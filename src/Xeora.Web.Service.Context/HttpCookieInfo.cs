using System;
using System.Globalization;

namespace Xeora.Web.Service.Context
{
    public class HttpCookieInfo : Basics.Context.IHttpCookieInfo
    {
        public HttpCookieInfo(string name) =>
            this.Name = name;

        public string Name { get; private set; }
        public string Value { get; set; }
        public DateTime Expires { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }

        public override string ToString()
        {
            string rCookie = 
                string.Format("{0}={1}", this.Name, System.Web.HttpUtility.UrlEncode(this.Value));

            if (!this.Expires.Equals(DateTime.MinValue))
            {
                string expires =
                    this.Expires.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
                rCookie = string.Format("{0}; expires={1}", rCookie, expires);
            }

            if (!string.IsNullOrEmpty(this.Domain))
                rCookie = string.Format("{0}; domain={1}", rCookie, this.Domain);

            if (string.IsNullOrEmpty(this.Path))
                this.Path = "/";
            rCookie = string.Format("{0}; path={1}", rCookie, this.Path);

            if (this.Secure)
                rCookie = string.Format("{0}; Secure", rCookie);

            if (this.HttpOnly)
                rCookie = string.Format("{0}; HttpOnly", rCookie);

            return rCookie;
        }
    }
}
