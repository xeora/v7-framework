using Newtonsoft.Json;
using System.ComponentModel;

namespace Xeora.Web.Configuration
{
    public class Parallelism : Basics.Configuration.IParallelism
    {
        private const ushort DEFAULT_MAX_CONNECTION = 128;
        private const ushort DEFAULT_MAGNITUDE = 4;
        private const ushort MAX_WORKER_THREADS = 2048;
        
        public Parallelism()
        {
            this.MaxConnection = Parallelism.DEFAULT_MAX_CONNECTION;
            this.Magnitude = Parallelism.DEFAULT_MAGNITUDE;
        }
        
        [DefaultValue(128)]
        [JsonProperty(PropertyName = "maxConnection", DefaultValueHandling = DefaultValueHandling.Populate)]
        public ushort MaxConnection { get; private set; }
        
        [DefaultValue(4)]
        [JsonProperty(PropertyName = "magnitude", DefaultValueHandling = DefaultValueHandling.Populate)]
        public ushort Magnitude { get; private set; }

        public ushort WorkerThreads
        {
            get
            {
                uint workerThreads =
                    (uint)(this.MaxConnection * this.Magnitude);
                
                if (workerThreads == 0)
                {
                    if (this.MaxConnection == 0 && this.Magnitude > 0)
                        workerThreads = (uint)(Parallelism.DEFAULT_MAX_CONNECTION * this.Magnitude);
                    else if (this.MaxConnection > 0 && this.Magnitude == 0)
                        workerThreads = (uint)(this.MaxConnection * Parallelism.DEFAULT_MAGNITUDE);
                }
                
                if (workerThreads is 0 or > Parallelism.MAX_WORKER_THREADS)
                    workerThreads = Parallelism.MAX_WORKER_THREADS;

                return (ushort)workerThreads;
            }
        }
    }
}
