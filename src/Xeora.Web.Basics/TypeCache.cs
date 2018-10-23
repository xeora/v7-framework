using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Basics
{
    internal class TypeCache
    {
        private static object _Lock = new object();
        private static TypeCache _Current = null;
        public static TypeCache Current
        {
            get
            {
                Monitor.Enter(TypeCache._Lock);
                try
                {
                    if (TypeCache._Current == null)
                        TypeCache._Current = new TypeCache();
                }
                finally
                {
                    Monitor.Exit(TypeCache._Lock);
                }

                return TypeCache._Current;
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
