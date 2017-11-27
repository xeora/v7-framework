using System.Collections.Generic;
using System.IO;

namespace Xeora.Extension.Executable
{
    public class Cache
    {
        private static Cache _self = null;
        private List<CacheInfo> _Cache;

        public Cache() =>
			this._Cache = new List<CacheInfo>();

        public static Cache Instance
        {
            get
            {
                if (Cache._self == null)
                    Cache._self = new Cache();

                return Cache._self;
            }
        }

        public bool IsLatest(string assemblyPath, QueryTypes queryType, string[] classIDs = null)
        {
            foreach (string assemblyFile in Directory.GetFiles(assemblyPath, "*.dll"))
            {
                FileInfo assemblyFileInfo = new FileInfo(assemblyFile);

                foreach (CacheInfo cI in this._Cache)
                {
                    if (string.Compare(cI.AssemblyFile, assemblyFile) == 0 && 
                        System.DateTime.Compare(cI.AssemblyDate, assemblyFileInfo.LastWriteTime) >= 0)
                    {
                        ClassInfo cO;

                        switch (queryType)
                        {
                            case QueryTypes.Classes:
                                if (classIDs != null)
                                {
                                    cO = cI.Find(classIDs);

                                    if (cO == null || 
                                        (cO != null && !cO.ClassesTouched))
                                        return false;
                                }

                                return cI.BaseClass.ClassesTouched;
                            case QueryTypes.Methods:
                                if (classIDs != null)
                                {
                                    cO = cI.Find(classIDs);

                                    if (cO == null || 
                                        (cO != null && !cO.MethodsTouched))
                                        return false;
                                }

                                return true;
                            default:
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public string[] GetIDs(string assemblyPath)
        {
            List<string> rStringList = new List<string>();

            foreach (CacheInfo cI in this._Cache)
            {
                if (string.Compare(assemblyPath, cI.AssemblyPath) == 0)
                    rStringList.Add(cI.AssemblyID);
            }

            return rStringList.ToArray();
        }

        public CacheInfo AddInfo(string assemblyPath, string assemblyID)
        {
            for (int cIC = this._Cache.Count - 1; cIC >= 0; cIC--)
            {
                CacheInfo aCI = this._Cache[cIC];

                if (string.Compare(aCI.AssemblyPath, assemblyPath) == 0 && 
                    string.Compare(aCI.AssemblyID, assemblyID) == 0)
                    this._Cache.RemoveAt(cIC);
            }

            CacheInfo rCacheInfo = 
                new CacheInfo(assemblyPath, assemblyID);
            this._Cache.Add(rCacheInfo);

            return rCacheInfo;
        }

        public CacheInfo GetInfo(string assemblyPath, string assemblyID)
        {
            foreach (CacheInfo Item in this._Cache)
            {
                if (string.Compare(Item.AssemblyPath, assemblyPath) == 0 && 
                    string.Compare(Item.AssemblyID, assemblyID) == 0)
                    return Item;
            }

            return null;
        }
    }
}
