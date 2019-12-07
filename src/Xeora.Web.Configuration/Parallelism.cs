using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Parallelism : Basics.Configuration.IParallelism
    {
        public Parallelism()
        {
            this.Worker = 4;
            this.WorkerThread = 8;
            this.Bucket = 32;
            this.BucketThread = 4;
        }
        
        [DefaultValue(4)]
        [JsonProperty(PropertyName = "worker", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short Worker { get; private set; }

        [DefaultValue(8)]
        [JsonProperty(PropertyName = "workerThread", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short WorkerThread { get; private set; }
        
        [DefaultValue(32)]
        [JsonProperty(PropertyName = "bucket", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short Bucket { get; private set; }

        [DefaultValue(4)]
        [JsonProperty(PropertyName = "bucketThread", DefaultValueHandling = DefaultValueHandling.Populate)]
        public short BucketThread { get; private set; }
    }
}
