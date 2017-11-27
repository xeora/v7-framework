using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Session : Basics.Configuration.ISession
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
