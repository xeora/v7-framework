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

        public DomainDeployment GetOrCreate(string[] domainIDAccessTree) =>
            this.GetOrCreate(domainIDAccessTree, null);

        public DomainDeployment GetOrCreate(string[] domainIDAccessTree, string languageID)
        {
            string instancenKey = 
                string.Format("{0}_{1}", string.Join<string>("-", domainIDAccessTree), languageID);

            DomainDeployment domainDeployment = null;
            if (!this._Instances.TryGetValue(instancenKey, out domainDeployment))
            {
                domainDeployment = new DomainDeployment(domainIDAccessTree, languageID);

                if (!this._Instances.TryAdd(instancenKey, domainDeployment))
                    return this.GetOrCreate(domainIDAccessTree, languageID);
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