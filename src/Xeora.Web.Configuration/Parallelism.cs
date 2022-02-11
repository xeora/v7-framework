using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Parallelism : Basics.Configuration.IParallelism
    {
        public Parallelism()
        {
            this.MaxConnection = 0;
        }
        
        [DefaultValue(0)]
        [JsonProperty(PropertyName = "maxConnection", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short MaxConnection { get; private set; }
    }
}
