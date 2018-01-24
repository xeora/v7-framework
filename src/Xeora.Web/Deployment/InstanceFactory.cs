using System.Collections.Concurrent;

namespace Xeora.Web.Deployment
{
    public sealed class InstanceFactory
    {
        private ConcurrentDictionary<string, DomainDeployment> _Instances;

        public InstanceFactory() =>
            this._Instances = new ConcurrentDictionary<string, DomainDeployment>();

        private static InstanceFactory _Current = null;
        public static InstanceFactory Current
        {
            get
            {
                if (InstanceFactory._Current == null)
                    InstanceFactory._Current = new InstanceFactory();

                return InstanceFactory._Current;
            }
        }

        public DomainDeployment GetOrCreate(string[] domainIDAccessTree)
        {
            string instancenKey = 
                string.Join<string>("-", domainIDAccessTree);

            DomainDeployment domainDeployment = null;
            if (!this._Instances.TryGetValue(instancenKey, out domainDeployment))
            {
                domainDeployment = new DomainDeployment(domainIDAccessTree);

                if (!this._Instances.TryAdd(instancenKey, domainDeployment))
                    return this.GetOrCreate(domainIDAccessTree);
            }

            return domainDeployment;
        }

        public void Reset()
        {
            foreach (string key in this._Instances.Keys)
            {
                DomainDeployment domainDeployment = null;

                this._Instances.TryRemove(key, out domainDeployment);

                if (domainDeployment != null)
                    domainDeployment.Dispose();
            }
        }

        ~InstanceFactory() =>
            this.Reset();
    }
}