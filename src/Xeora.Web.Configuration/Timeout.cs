using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Timeout : Basics.Configuration.ITimeout
    {
        private const uint DEFAULT_READ_TIMEOUT = 30000;
        private const uint DEFAULT_WRITE_TIMEOUT = 30000;

        public Timeout()
        {
            this.Read = Timeout.DEFAULT_READ_TIMEOUT;
            this.Write = Timeout.DEFAULT_WRITE_TIMEOUT;
        }
        
        [DefaultValue(30000)]
        [JsonProperty(PropertyName = "read", DefaultValueHandling = DefaultValueHandling.Populate)]
        public uint Read { get; private set; }
        
        [DefaultValue(30000)]
        [JsonProperty(PropertyName = "write", DefaultValueHandling = DefaultValueHandling.Populate)]
        public uint Write { get; private set; }
    }
}
