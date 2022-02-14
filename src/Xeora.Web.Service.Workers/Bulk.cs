using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    public class Bulk
    {
        private readonly ConcurrentDictionary<string, ActionContainer> _ActionQueueContainers;
        private readonly ConcurrentDictionary<string, ActionContainer> _ActionTrackerContainers;
        private readonly Action<ActionContainer> _AddHandler;
        
        private readonly object _Lock = new();

        internal Bulk(Action<ActionContainer> addHandler)
        {
            this._ActionQueueContainers = new ConcurrentDictionary<string, ActionContainer>();
            this._ActionTrackerContainers = new ConcurrentDictionary<string, ActionContainer>();
            this._AddHandler = addHandler;
        }

        private void Completed(string id)
        {
            Monitor.Enter(this._Lock);
            try
            {
                this._ActionQueueContainers.TryRemove(id, out _);
                if (!this._ActionQueueContainers.IsEmpty) return;
                
                Monitor.Pulse(this._Lock);
            }
            finally
            {
                Monitor.Exit(this._Lock);
            }
        }

        public void Add(Action<object> startHandler, object state, ActionType actionType)
        {
            ActionContainer actionContainer =
                new ActionContainer(startHandler, state, actionType, this.Completed);
            
            this._ActionQueueContainers.TryAdd(actionContainer.Id, actionContainer);
            this._ActionTrackerContainers.TryAdd(actionContainer.Id, actionContainer);
        }

        private void ScheduleOrRun(ActionType actionType)
        {
            // Prioritized the actions
            foreach (var (_, actionContainer) in this._ActionTrackerContainers)
            {
                if (actionContainer.Type != actionType) continue;

                if (Factory.Available)
                {
                    this._AddHandler.Invoke(actionContainer);
                    continue;
                }
                
                actionContainer.Invoke();
            }
        }
        
        public void Process()
        {
            // Check if bulk is empty
            if (this._ActionQueueContainers.IsEmpty) return;

            // Prioritized the actions
            this.ScheduleOrRun(ActionType.Attached);
            this.ScheduleOrRun(ActionType.External);

            // Check bulk actions handled as sync and no more bulk to wait 
            if (this._ActionQueueContainers.IsEmpty) return;
            
            Monitor.Enter(this._Lock);
            try
            {
                // Check until it reaches here, all bulk actions are concluded
                if (this._ActionQueueContainers.IsEmpty) return;
                
                Monitor.Wait(this._Lock);
            }
            finally
            {
                Monitor.Exit(this._Lock);
            }
        }
    }
}
