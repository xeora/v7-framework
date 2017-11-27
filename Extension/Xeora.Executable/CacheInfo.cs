using System.IO;

namespace Xeora.Extension.Executable
{
    public class CacheInfo
    {
        private System.DateTime _AssemblyDate;
        private ClassInfo _ClassInfo;

        public CacheInfo(string assemblyPath, string assemblyID)
        {
            this.AssemblyPath = assemblyPath;
            this.AssemblyID = assemblyID;

            this.AssemblyFile = 
                Path.Combine(this.AssemblyPath, string.Format("{0}.dll", this.AssemblyID));

            if (File.Exists(this.AssemblyFile))
                this._AssemblyDate = File.GetLastWriteTime(this.AssemblyFile);
            else
                this._AssemblyDate = System.DateTime.MaxValue;

            this._ClassInfo = new ClassInfo(assemblyID);
        }

        public string AssemblyPath { get; private set; }
        public string AssemblyID { get; private set; }
        public string AssemblyFile { get; private set; }
        public System.DateTime AssemblyDate { get; private set; }
        public ClassInfo BaseClass { get; private set; }

        public ClassInfo Find(string[] classIDs)
        {
            foreach (string classID in classIDs)
            {
                foreach (ClassInfo classInfo in this.BaseClass.Classes)
                {
                    if (string.Compare(classInfo.ID, classID) == 0)
                        return classInfo;
                }
            }

            return this.BaseClass;
        }
    }
}
