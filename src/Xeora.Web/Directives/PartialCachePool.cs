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
                    return PartialCachePool._Current ?? (PartialCachePool._Current = new PartialCachePool());
                }
                finally
                {
                    Monitor.Exit(PartialCachePool.Lock);
                }
            }
        }

        public void AddOrUpdate(string[] domainIdAccessTree, PartialCacheObject cacheObject)
        {
            if (!this._PartialCaches.TryGetValue(domainIdAccessTree, out ConcurrentDictionary<string, PartialCacheObject> cacheObjects))
            {
                cacheObjects = 
                    new ConcurrentDictionary<string, PartialCacheObject>();
                this._PartialCaches.TryAdd(domainIdAccessTree, cacheObjects);
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

        public void Reset() =>
            this._PartialCaches.Clear();
    }
}