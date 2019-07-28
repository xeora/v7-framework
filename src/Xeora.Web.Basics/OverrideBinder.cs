using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Xeora.Web.Basics
{
    internal class OverrideBinder : SerializationBinder
    {
        private static readonly ConcurrentDictionary<string, Assembly> AssemblyCache = 
            new ConcurrentDictionary<string, Assembly>();

        public override Type BindToType(string assemblyName, string typeName)
        {
            string sShortAssemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));

            if (OverrideBinder.AssemblyCache.TryGetValue(sShortAssemblyName, out Assembly assembly))
                return this.GetDeserializeType(assembly, typeName);

            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Substring(0, assemblyName.IndexOf(',')))
                {
                    OverrideBinder.AssemblyCache.TryAdd(sShortAssemblyName, ayAssembly);

                    return this.GetDeserializeType(ayAssembly, typeName);
                }
            }

            return null;
        }

        private Type GetDeserializeType(Assembly assembly, string typeName)
        {
            string typeNameL = 
                this.GetTypeFullNames(typeName, out string[] remainAssemblyNames);

            Type tempType = assembly.GetType(typeNameL);

            if (tempType == null || !tempType.IsGenericType) return tempType;
            
            List<Type> typeParameters = new List<Type>();

            foreach (string remainAssemblyName in remainAssemblyNames)
            {
                int eBI = remainAssemblyName.LastIndexOf(']');
                string qAssemblyName, qTypeName;

                if (eBI == -1)
                {
                    qTypeName = remainAssemblyName.Split(',')[0];
                    qAssemblyName = remainAssemblyName.Substring(qTypeName.Length + 2);
                }
                else
                {
                    qTypeName = remainAssemblyName.Substring(0, eBI + 1);
                    qAssemblyName = remainAssemblyName.Substring(eBI + 3);
                }

                typeParameters.Add(this.BindToType(qAssemblyName, qTypeName));
            }

            return tempType.MakeGenericType(typeParameters.ToArray());

        }

        private string GetTypeFullNames(string typeName, out string[] remainAssemblyNames)
        {
            int bI = typeName.IndexOf('[', 0);

            if (bI == -1)
            {
                remainAssemblyNames = new string[] { };

                return typeName;
            }

            List<string> fullNameListL = new List<string>();
            string remainFullName = typeName.Substring(bI + 1, typeName.Length - (bI + 1) - 1);

            bI = 0;
            do
            {
                bI = remainFullName.IndexOf('[', bI);

                if (bI <= -1) continue;
                
                int eI = remainFullName.IndexOf(']', bI + 1);
                int bIc = remainFullName.IndexOf('[', bI + 1);

                if (bIc > -1 && bIc < eI)
                {
                    while (bIc > -1 && bIc < eI)
                    {
                        bIc = remainFullName.IndexOf('[', bIc + 1);

                        if (bIc > -1 && bIc < eI)
                            eI = remainFullName.IndexOf(']', eI + 1);
                    }

                    eI = remainFullName.IndexOf(']', eI + 1);
                }

                fullNameListL.Add(remainFullName.Substring(bI + 1, eI - (bI + 1)));

                bI = eI + 1;
            } while (bI != -1);

            remainAssemblyNames = fullNameListL.ToArray();

            return typeName.Substring(0, bI);
        }
    }
}
