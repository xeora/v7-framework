using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Xeora.Web.Basics
{
    internal class OverrideBinder : SerializationBinder
    {
        private static ConcurrentDictionary<string, Assembly> _AssemblyCache = 
            new ConcurrentDictionary<string, Assembly>();

        public override Type BindToType(string assemblyName, string typeName)
        {
            string sShortAssemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));

            if (OverrideBinder._AssemblyCache.TryGetValue(sShortAssemblyName, out Assembly assembly))
                return this.GetDeserializeType(assembly, typeName);

            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Substring(0, assemblyName.IndexOf(',')))
                {
                    OverrideBinder._AssemblyCache.TryAdd(sShortAssemblyName, ayAssembly);

                    return this.GetDeserializeType(ayAssembly, typeName);
                }
            }

            return null;
        }

        private Type GetDeserializeType(Assembly assembly, string typeName)
        {
            string[] remainAssemblyNames = null;
            string typeName_L = this.GetTypeFullNames(typeName, ref remainAssemblyNames);

            Type tempType = assembly.GetType(typeName_L);

            if (tempType != null && tempType.IsGenericType)
            {
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

            return tempType;
        }

        private string GetTypeFullNames(string typeName, ref string[] remainAssemblyNames)
        {
            int bI = typeName.IndexOf('[', 0);

            if (bI == -1)
            {
                remainAssemblyNames = new string[] { };

                return typeName;
            }

            List<string> fullNameList_L = new List<string>();
            string remainFullName = typeName.Substring(bI + 1, typeName.Length - (bI + 1) - 1);

            int eI = 0, bIc = 0;
            bI = 0;
            do
            {
                bI = remainFullName.IndexOf('[', bI);

                if (bI > -1)
                {
                    eI = remainFullName.IndexOf(']', bI + 1);
                    bIc = remainFullName.IndexOf('[', bI + 1);

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

                    fullNameList_L.Add(remainFullName.Substring(bI + 1, eI - (bI + 1)));

                    bI = eI + 1;
                }
            } while (bI != -1);

            remainAssemblyNames = fullNameList_L.ToArray();

            return typeName.Substring(0, bI);
        }
    }
}
