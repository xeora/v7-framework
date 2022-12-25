using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization;

namespace Xeora.Web.Tools.Serialization
{
    internal class Binder : SerializationBinder
    {
        private readonly string _ContextName;
        public Binder(string contextName) => 
            this._ContextName = contextName;
        
        public override Type BindToType(string assemblyName, string typeName)
        {
            foreach (AssemblyLoadContext context in AssemblyLoadContext.All)
            {
                if (string.CompareOrdinal(context.Name, this._ContextName) != 0) continue;

                foreach (Assembly assembly in context.Assemblies)
                {
                    if (string.CompareOrdinal(assembly.FullName, assemblyName) != 0) continue;

                    return assembly.GetType(typeName);
                }
            }

            return null;
        }
    }
}
