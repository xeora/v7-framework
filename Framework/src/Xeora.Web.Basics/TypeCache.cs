using System;
using System.Reflection;
using System.Collections.Concurrent;

namespace Xeora.Web.Basics
{
    internal class TypeCache
    {
        private static TypeCache _Instance = null;
        public static TypeCache Instance
        {
            get
            {
                if (TypeCache._Instance == null)
                    TypeCache._Instance = new TypeCache();

                return TypeCache._Instance;
            }
        }

        private ConcurrentDictionary<string, Assembly> _LoadedAssemblies =
            new ConcurrentDictionary<string, Assembly>();
        private Assembly GetAssembly(string assemblyID)
        {
            Assembly rAssembly = null;

            if (!this._LoadedAssemblies.TryGetValue(assemblyID, out rAssembly))
            {
                rAssembly = Assembly.Load(assemblyID);
                this._LoadedAssemblies.TryAdd(assemblyID, rAssembly);
            }

            return rAssembly;
        }

        private Type _RemoteInvokeType = null;
        public Type RemoteInvoke
        {
            get
            {
                if (this._RemoteInvokeType != null)
                    return this._RemoteInvokeType;

                Assembly LoadedAssembly = this.GetAssembly("Xeora.Web.Handler");
                this._RemoteInvokeType = LoadedAssembly.GetType("Xeora.Web.Handler.RemoteInvoke", false, true);

                return this._RemoteInvokeType;
            }
        }

        private Type _DomainType = null;
        public Type Domain
        {
            get
            {
                if (this._DomainType != null)
                    return this._DomainType;

                Assembly LoadedAssembly = this.GetAssembly("Xeora.Web");
                this._DomainType = LoadedAssembly.GetType("Xeora.Web.Site.Domain", false, true);

                return this._DomainType;
            }
        }
    }
}
