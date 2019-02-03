using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Deployment
{
    public sealed class InstanceFactory
    {
        private ConcurrentDictionary<string, Domain> _Instances;

        public InstanceFactory() =>
            this._Instances = new ConcurrentDictionary<string, Domain>();

        private static object _Lock = new object();
        private static InstanceFactory _Current = null;
        public static InstanceFactory Current
        {
            get
            {
                Monitor.Enter(InstanceFactory._Lock);
                try
                {
                    if (InstanceFactory._Current == null)
                        InstanceFactory._Current = new InstanceFactory();
                }
                finally
                {
                    Monitor.Exit(InstanceFactory._Lock);
                }

                return InstanceFactory._Current;
            }
        }

        public Domain GetOrCreate(string[] domainIDAccessTree)
        {
            string instancenKey = 
                string.Join<string>("-", domainIDAccessTree);

            Domain domain = null;
            if (!this._Instances.TryGetValue(instancenKey, out domain))
            {
                domain = new Domain(domainIDAccessTree);

                if (!this._Instances.TryAdd(instancenKey, domain))
                    return this.GetOrCreate(domainIDAccessTree);
            } 
            else
                domain.Reload();

            return domain;
        }

        public void Reset()
        {
            foreach (string key in this._Instances.Keys)
            {
                this._Instances.TryRemove(key, out Domain domain);

                if (domain != null)
                    domain.Dispose();
            }
        }

        ~InstanceFactory() => this.Reset();
    }
}