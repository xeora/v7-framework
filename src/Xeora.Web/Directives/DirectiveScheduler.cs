using System;
using System.Collections.Concurrent;

namespace Xeora.Web.Directives
{
    public class DirectiveScheduler
    {
        private readonly object _RegisterLock;

        private readonly ConcurrentDictionary<string, bool> _ScheduledItems;
        private readonly ConcurrentQueue<string> _Queue;
        
        private readonly Action<string> _CallBack;

        public DirectiveScheduler(Action<string> callback)
        {
            this._RegisterLock = new object();

            this._ScheduledItems = 
                new ConcurrentDictionary<string, bool>();
            this._Queue = 
                new ConcurrentQueue<string>();
            
            this._CallBack = callback;
        }

        public void Register(string uniqueId)
        {
            lock (this._RegisterLock)
            {
                if (this._ScheduledItems.ContainsKey(uniqueId))
                    return;
                this._ScheduledItems.TryAdd(uniqueId, true);
                this._Queue.Enqueue(uniqueId);
            }
        }

        public void Fire()
        {
            while (this._Queue.TryDequeue(out string uniqueId))
            {
                this._ScheduledItems.TryRemove(uniqueId, out _);
                this._CallBack(uniqueId);
            }
        }
    }
}
