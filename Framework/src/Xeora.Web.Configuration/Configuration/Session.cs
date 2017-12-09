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
            this.ServiceType = SessionServiceTypes.BuiltIn;
            this.CookieKey = "xcsid";
            this.Timeout = 20;
        }

        [DefaultValue(SessionServiceTypes.BuiltIn)]
        [JsonProperty(PropertyName = "serviceType", DefaultValueHandling = DefaultValueHandling.Populate)]
        public SessionServiceTypes ServiceType { get; private set; }

        [DefaultValue("127.0.0.1:5531")]
        [JsonProperty(PropertyName = "serviceEndPoint", DefaultValueHandling = DefaultValueHandling.Populate)]
        private string _ServiceEndPoint { get; set; }

        public IPEndPoint ServiceEndPoint
        {
            get
            {
                if (string.IsNullOrEmpty(this._ServiceEndPoint))
                    this._ServiceEndPoint = "127.0.0.1:5531";

                int colonIndex = this._ServiceEndPoint.IndexOf(':');
                if (colonIndex == -1)
                    this._ServiceEndPoint = string.Format("{0}:5531", this._ServiceEndPoint);

                IPAddress serviceIP;
                if (!IPAddress.TryParse(this._ServiceEndPoint.Split(':')[0], out serviceIP))
                    serviceIP = IPAddress.Parse("127.0.0.1");

                int servicePort;
                if (!int.TryParse(this._ServiceEndPoint.Split(':')[1], out servicePort))
                    servicePort = 5531;
                
                return new IPEndPoint(serviceIP, servicePort);
            }
        }

        [DefaultValue("xcsid")]
        [JsonProperty(PropertyName = "cookieKey", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CookieKey { get; private set; }

        [DefaultValue(20)]
        [JsonProperty(PropertyName = "timeout", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short Timeout { get; private set; }
    }
}
