using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;

namespace Xeora.Web.Configuration
{
    public class Service : Basics.Configuration.IService
    {
        public Service()
        {
            this._Address = "127.0.0.1";
            this.Port = 3381;
            this.Ssl = false;
            this.CertificatePassword = string.Empty;
            this.Print = false;
        }

        [DefaultValue("127.0.0.1")]
        [JsonProperty(PropertyName = "address", DefaultValueHandling = DefaultValueHandling.Populate)]
        private string _Address { get; set; }

        public IPAddress Address => IPAddress.Parse(this._Address);

        [DefaultValue(3381)]
        [JsonProperty(PropertyName = "port", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short Port { get; private set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "ssl", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Ssl { get; private set; }

        [DefaultValue("")]
        [JsonProperty(PropertyName = "certificatePassword", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CertificatePassword { get; private set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "print", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Print { get; private set; }
    }
}
