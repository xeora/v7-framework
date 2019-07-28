using Newtonsoft.Json;

namespace Xeora.Web.Configuration.ApplicationSections
{
    public class MimeItem : Basics.Configuration.IMimeItem
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public string Type { get; private set; }

        [JsonProperty(PropertyName = "extension", Required = Required.Always)]
        public string Extension { get; private set; }
    }
}
