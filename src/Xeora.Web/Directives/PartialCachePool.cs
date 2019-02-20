using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Directives
{
    internal class PartialCachePool
    {
        private readonly ConcurrentDictionary<string[], ConcurrentDictionary<string, PartialCacheObject>> _PartialCaches;

        public PartialCachePool() =>
            this._PartialCaches = new ConcurrentDictionary<string[], ConcurrentDictionary<string, PartialCacheObject>>();

        private static readonly object _Lock = new object();
        private static PartialCachePool _Current = null;
        public static PartialCachePool Current
        {
            get
            {
                Monitor.Enter(PartialCachePool._Lock);
                try
                {
                    if (PartialCachePool._Current == null)
                        PartialCachePool._Current = new PartialCachePool();
                }
                finally
                {
                    Monitor.Exit(PartialCachePool._Lock);
                }

                return PartialCachePool._Current;
            }
        }

        public void AddOrUpdate(string[] domainIDAccessTree, PartialCacheObject cacheObject)
        {
            if (!this._PartialCaches.TryGetValue(domainIDAccessTree, out ConcurrentDictionary<string, PartialCacheObject> cacheObjects))
            {
                cacheObjects = new ConcurrentDictionary<string, PartialCacheObject>();

                if (!this._PartialCaches.TryAdd(domainIDAccessTree, cacheObjects))
                {
                    this.AddOrUpdate(domainIDAccessTree, cacheObject);

                    return;
                }
            }

            cacheObjects.AddOrUpdate(cacheObject.CacheID, cacheObject, (cCacheID, cCacheObject) => cacheObject);
        }

        public void Get(string[] domainIDAccessTree, string cacheID, out PartialCacheObject cacheObject)
        {
            cacheObject = null;

            if (!this._PartialCaches.TryGetValue(domainIDAccessTree, out ConcurrentDictionary<string, PartialCacheObject> cacheObjects))
                return;

            cacheObjects.TryGetValue(cacheID, out cacheObject);
        }

        public void Reset(string[] domainIDAccessTree) =>
            this._PartialCaches.TryRemove(domainIDAccessTree, out ConcurrentDictionary<string, PartialCacheObject> dummy);
    }
}