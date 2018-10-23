using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using Xeora.Web.Basics.Configuration;

namespace Xeora.Web.Configuration
{
    public class Session : ISession
    {
        public Session()
        {
            this.CookieKey = "xcsid";
            this.Timeout = 20;
        }

        [DefaultValue("xcsid")]
        [JsonProperty(PropertyName = "cookieKey", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CookieKey { get; private set; }

        [DefaultValue(20)]
        [JsonProperty(PropertyName = "timeout", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short Timeout { get; private set; }
    }
}
