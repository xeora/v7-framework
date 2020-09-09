using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Deployment
{
    public sealed class InstanceFactory
    {
        private readonly ConcurrentDictionary<string, Domain> _Instances;

        private InstanceFactory() =>
            this._Instances = new ConcurrentDictionary<string, Domain>();

        private static readonly object Lock = new object();
        private static InstanceFactory _current;
        public static InstanceFactory Current
        {
            get
            {
                Monitor.Enter(InstanceFactory.Lock);
                try
                {
                    return InstanceFactory._current ?? (InstanceFactory._current = new InstanceFactory());
                }
                finally
                {
                    Monitor.Exit(InstanceFactory.Lock);
                }
            }
        }

        public Domain GetOrCreate(string[] domainIdAccessTree)
        {
            string instanceKey = 
                string.Join<string>("-", domainIdAccessTree);

            if (this._Instances.TryGetValue(instanceKey, out Domain domain)) 
                return domain;
            
            domain = 
                new Domain(domainIdAccessTree);
            this._Instances.TryAdd(instanceKey, domain);

            return domain;
        }

        public void Reset()
        {
            string[] keys = 
                new string[this._Instances.Keys.Count];
            this._Instances.Keys.CopyTo(keys, 0);
            
            foreach (string key in keys)
            {
                if (this._Instances.TryRemove(key, out Domain domain))
                    domain.Dispose();
            }
        }

        ~InstanceFactory() => this.Reset();
    }
}