using System;

namespace Xeora.Web.Global
{
    [Serializable()]
    public class DataListOutputInfo
    {
        public DataListOutputInfo(string uniqueID, long count, long total)
        {
            this.UniqueID = uniqueID;
            this.Count = count;
            this.Total = total;
        }

        public string UniqueID { get; private set; }
        public long Count { get; private set; }
        public long Total { get; private set; }
    }
}