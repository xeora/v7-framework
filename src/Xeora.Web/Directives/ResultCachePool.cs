using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Directives
{
    internal class ResultCachePool
    {
        private readonly ConcurrentDictionary<string[], ConcurrentDictionary<string, ResultCacheObject>> _ResultCaches;

        private ResultCachePool() =>
            this._ResultCaches = new ConcurrentDictionary<string[], ConcurrentDictionary<string, ResultCacheObject>>();

        private static readonly object Lock = new object();
        private static ResultCachePool _current;
        public static ResultCachePool Current
        {
            get
            {
                Monitor.Enter(ResultCachePool.Lock);
                try
                {
                    return ResultCachePool._current ?? (ResultCachePool._current = new ResultCachePool());
                }
                finally
                {
                    Monitor.Exit(ResultCachePool.Lock);
                }
            }
        }

        public void AddOrUpdate(string[] domainIdAccessTree, ResultCacheObject cacheObject)
        {
            if (!this._ResultCaches.TryGetValue(domainIdAccessTree, out ConcurrentDictionary<string, ResultCacheObject> cacheObjects))
            {
                cacheObjects = 
                    new ConcurrentDictionary<string, ResultCacheObject>();
                this._ResultCaches.TryAdd(domainIdAccessTree, cacheObjects);
            }

            cacheObjects.AddOrUpdate(cacheObject.CacheId, cacheObject, (cCacheId, cCacheObject) => cacheObject);
        }

        public void Get(string[] domainIdAccessTree, string cacheId, out ResultCacheObject cacheObject)
        {
            cacheObject = null;

            if (!this._ResultCaches.TryGetValue(domainIdAccessTree, out ConcurrentDictionary<string, ResultCacheObject> cacheObjects))
                return;

            cacheObjects.TryGetValue(cacheId, out cacheObject);
        }

        public void Reset() =>
            this._ResultCaches.Clear();
    }
}