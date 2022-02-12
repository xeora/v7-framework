using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Parallelism : Basics.Configuration.IParallelism
    {
        public Parallelism()
        {
            this.MaxConnection = 128;
        }
        
        [DefaultValue(128)]
        [JsonProperty(PropertyName = "maxConnection", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short MaxConnection { get; private set; }
    }
}
