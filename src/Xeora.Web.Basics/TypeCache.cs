using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Basics
{
    internal class TypeCache
    {
        private static object _Lock = new object();
        private static TypeCache _Current;
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
        private Assembly GetAssembly(string assemblyId)
        {
            if (!this._LoadedAssemblies.TryGetValue(assemblyId, out Assembly rAssembly))
            {
                rAssembly = Assembly.Load(assemblyId);
                this._LoadedAssemblies.TryAdd(assemblyId, rAssembly);
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

        private Type _RenderEngineType = null;
        public Type RenderEngine
        {
            get
            {
                if (this._RenderEngineType != null)
                    return this._RenderEngineType;

                Assembly LoadedAssembly = this.GetAssembly("Xeora.Web");
                this._RenderEngineType = LoadedAssembly.GetType("Xeora.Web.Site.RenderEngine", false, true);

                return this._RenderEngineType;
            }
        }

        private Type _StatusTracker = null;
        public Type StatusTracker
        {
            get
            {
                if (this._StatusTracker != null)
                    return this._StatusTracker;

                Assembly LoadedAssembly = this.GetAssembly("Xeora.Web.Service");
                this._StatusTracker = LoadedAssembly.GetType("Xeora.Web.Service.StatusTracker", false, true);

                return this._StatusTracker;
            }
        }
    }
}
