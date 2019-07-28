using System;

namespace Xeora.Web.Global
{
    [Serializable]
    public class DataListOutputInfo
    {
        public DataListOutputInfo(string uniqueId, long count, long total, bool failed)
        {
            this.UniqueId = uniqueId;
            this.Count = count;
            this.Total = total;
            this.Failed = failed;
        }

        public string UniqueId { get; }
        public long Count { get; }
        public long Total { get; }
        public bool Failed { get; }
    }
}