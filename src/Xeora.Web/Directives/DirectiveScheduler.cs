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

            this._ScheduledItems = new ConcurrentDictionary<string, bool>();
            this._Queue = new ConcurrentQueue<string>();

            this._CallBack = callback;
        }

        public void Register(string uniqueID)
        {
            lock (this._RegisterLock)
            {
                if (this._ScheduledItems.ContainsKey(uniqueID))
                    return;

                this._Queue.Enqueue(uniqueID);
            }
        }

        public void Fire()
        {
            string handlerID = Helpers.CurrentHandlerID;
            List<Task> callbackJobs = new List<Task>();

            while (this._Queue.TryDequeue(out string uniqueID))
            {
                callbackJobs.Add(
                    Task.Factory.StartNew(() =>
                    {
                        Helpers.AssignHandlerID(handlerID);

                        this._CallBack(uniqueID);
                    })
                );
            }

            Task.WaitAll(callbackJobs.ToArray());
        }
    }
}
