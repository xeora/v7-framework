using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xeora.Web.Basics;

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

                this._Queue.Enqueue(uniqueId);
            }
        }

        public void Fire()
        {
            string handlerId = Helpers.CurrentHandlerId;
            List<Task> callbackJobs = new List<Task>();

            while (this._Queue.TryDequeue(out string uniqueId))
            {
                if (this._ScheduledItems.ContainsKey(uniqueId))
                    this._ScheduledItems.TryRemove(uniqueId, out bool _);
                
                string uId = uniqueId;
                
                callbackJobs.Add(
                    Task.Factory.StartNew(() =>
                    {
                        Helpers.AssignHandlerId(handlerId);

                        this._CallBack(uId);
                    })
                );
            }

            Task.WaitAll(callbackJobs.ToArray());
        }
    }
}
