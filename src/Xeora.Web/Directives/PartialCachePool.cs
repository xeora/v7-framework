using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Directives
{
    internal class PartialCachePool
    {
        private readonly ConcurrentDictionary<string[], ConcurrentDictionary<string, PartialCacheObject>> _PartialCaches;

        private PartialCachePool() =>
            this._PartialCaches = new ConcurrentDictionary<string[], ConcurrentDictionary<string, PartialCacheObject>>();

        private static readonly object Lock = new object();
        private static PartialCachePool _Current;
        public static PartialCachePool Current
        {
            get
            {
                Monitor.Enter(PartialCachePool.Lock);
                try
                {
                    if (PartialCachePool._Current == null)
                        PartialCachePool._Current = new PartialCachePool();
                }
                finally
                {
                    Monitor.Exit(PartialCachePool.Lock);
                }

                return PartialCachePool._Current;
            }
        }

        public void AddOrUpdate(string[] domainIdAccessTree, PartialCacheObject cacheObject)
        {
            if (!this._PartialCaches.TryGetValue(domainIdAccessTree, out ConcurrentDictionary<string, PartialCacheObject> cacheObjects))
            {
                cacheObjects = new ConcurrentDictionary<string, PartialCacheObject>();

                if (!this._PartialCaches.TryAdd(domainIdAccessTree, cacheObjects))
                {
                    this.AddOrUpdate(domainIdAccessTree, cacheObject);

                    return;
                }
            }

            cacheObjects.AddOrUpdate(cacheObject.CacheId, cacheObject, (cCacheId, cCacheObject) => cacheObject);
        }

        public void Get(string[] domainIdAccessTree, string cacheId, out PartialCacheObject cacheObject)
        {
            cacheObject = null;

            if (!this._PartialCaches.TryGetValue(domainIdAccessTree, out ConcurrentDictionary<string, PartialCacheObject> cacheObjects))
                return;

            cacheObjects.TryGetValue(cacheId, out cacheObject);
        }

        public void Reset(string[] domainIdAccessTree) =>
            this._PartialCaches.TryRemove(domainIdAccessTree, out ConcurrentDictionary<string, PartialCacheObject> dummy);
    }
}