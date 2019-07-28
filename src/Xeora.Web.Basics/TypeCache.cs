using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Basics
{
    internal class TypeCache
    {
        private static readonly object Lock = new object();
        private static TypeCache _Current;
        public static TypeCache Current
        {
            get
            {
                Monitor.Enter(TypeCache.Lock);
                try
                {
                    if (TypeCache._Current == null)
                        TypeCache._Current = new TypeCache();
                }
                finally
                {
                    Monitor.Exit(TypeCache.Lock);
                }

                return TypeCache._Current;
            }
        }

        private readonly ConcurrentDictionary<string, Assembly> _LoadedAssemblies =
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

        private Type _RemoteInvokeType;
        public Type RemoteInvoke
        {
            get
            {
                if (this._RemoteInvokeType != null)
                    return this._RemoteInvokeType;

                Assembly loadedAssembly = 
                    this.GetAssembly("Xeora.Web.Handler");
                this._RemoteInvokeType = 
                    loadedAssembly.GetType("Xeora.Web.Handler.RemoteInvoke", false, true);

                return this._RemoteInvokeType;
            }
        }

        private Type _DomainType;
        public Type Domain
        {
            get
            {
                if (this._DomainType != null)
                    return this._DomainType;

                Assembly loadedAssembly = 
                    this.GetAssembly("Xeora.Web");
                this._DomainType = 
                    loadedAssembly.GetType("Xeora.Web.Application.Domain.Domain", false, true);

                return this._DomainType;
            }
        }

        // TODO: RenderEngine should be fixed
        private Type _ParseType;
        public Type Parser
        {
            get
            {
                if (this._ParseType != null)
                    return this._ParseType;

                Assembly loadedAssembly = 
                    this.GetAssembly("Xeora.Web");
                this._ParseType = 
                    loadedAssembly.GetType("Xeora.Web.Application.Domain.Parser", false, true);

                return this._ParseType;
            }
        }

        private Type _StatusTracker;
        public Type StatusTracker
        {
            get
            {
                if (this._StatusTracker != null)
                    return this._StatusTracker;

                Assembly loadedAssembly = 
                    this.GetAssembly("Xeora.Web.Service");
                this._StatusTracker = 
                    loadedAssembly.GetType("Xeora.Web.Service.StatusTracker", false, true);

                return this._StatusTracker;
            }
        }
    }
}
