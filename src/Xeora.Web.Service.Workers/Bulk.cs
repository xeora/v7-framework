using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    public class Bulk
    {
        private readonly ConcurrentDictionary<string, ActionContainer> _ActionContainers;
        private readonly Action<ActionContainer> _AddHandler;
        
        private readonly object _Lock = new();

        internal Bulk(Action<ActionContainer> addHandler)
        {
            this._ActionContainers = new ConcurrentDictionary<string, ActionContainer>();
            this._AddHandler = addHandler;
        }

        private void Completed(string id)
        {
            this._ActionContainers.TryRemove(id, out _);
            if (!this._ActionContainers.IsEmpty) return;
            
            Monitor.Enter(this._Lock);
            try
            {
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
            this._ActionContainers.TryAdd(actionContainer.Id, actionContainer);
        }

        private void ScheduleOrRun(ActionType actionType, ref List<string> cleanupIdList)
        {
            // Prioritized the actions
            foreach (var (id, actionContainer) in this._ActionContainers)
            {
                if (actionContainer.Type != actionType) continue;

                if (Factory.Available)
                {
                    this._AddHandler.Invoke(actionContainer);
                    continue;
                }
                
                actionContainer.Invoke();
                cleanupIdList.Add(id);
            }
        }
        
        public void Process()
        {
            // Check if bulk is empty
            if (this._ActionContainers.IsEmpty) return;

            List<string> cleanupIdList = new List<string>();
            
            // Prioritized the actions
            this.ScheduleOrRun(ActionType.Attached, ref cleanupIdList);
            this.ScheduleOrRun(ActionType.External, ref cleanupIdList);

            while (cleanupIdList.Count > 0)
            {
                this._ActionContainers.TryRemove(cleanupIdList[0], out _);
                cleanupIdList.RemoveAt(0);
            }
            // Check bulk actions handled as sync and no more bulk to wait 
            if (this._ActionContainers.IsEmpty) return;
            
            Monitor.Enter(this._Lock);
            try
            {
                // Check until it reaches here, all bulk actions are concluded
                if (this._ActionContainers.IsEmpty) return;
                    
                Monitor.Wait(this._Lock);
            }
            finally
            {
                Monitor.Exit(this._Lock);
            }
        }
    }
}
