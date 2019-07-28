using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration.ApplicationSections
{
    public class RequestTagFilter : Basics.Configuration.IRequestTagFilter
    {
        public RequestTagFilter()
        {
            this.Direction = Basics.Configuration.RequestTagFilteringTypes.None;
            this.Items = new [] { "&gt;script" };
        }

        [DefaultValue(Basics.Configuration.RequestTagFilteringTypes.None)]
        [JsonProperty(PropertyName = "direction", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Populate)]
        public Basics.Configuration.RequestTagFilteringTypes Direction { get; private set; }

        [DefaultValue(new [] { "&gt;script" })]
        [JsonProperty(PropertyName = "items", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string[] Items { get; private set; }

        [JsonProperty(PropertyName = "exceptions")]
        public string[] Exceptions { get; private set; }
    }
}
