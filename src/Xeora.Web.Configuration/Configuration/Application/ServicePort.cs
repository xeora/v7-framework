using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class ServicePort : Basics.Configuration.IServicePort
    {
        public ServicePort()
        {
            this.VariablePool = 12005;
            this.ScheduledTasks = 0;
        }

        [DefaultValue(12005)]
        [JsonProperty(PropertyName = "variablePool", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short VariablePool { get; private set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "scheduledTasks", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short ScheduledTasks { get; private set; }
    }
}
