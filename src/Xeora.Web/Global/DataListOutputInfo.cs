using System;

namespace Xeora.Web.Global
{
    [Serializable]
    public class DataListOutputInfo
    {
        public DataListOutputInfo(string uniqueID, long count, long total, bool failed)
        {
            this.UniqueID = uniqueID;
            this.Count = count;
            this.Total = total;
            this.Failed = failed;
        }

        public string UniqueID { get; private set; }
        public long Count { get; private set; }
        public long Total { get; private set; }
        public bool Failed { get; private set; }
    }
}