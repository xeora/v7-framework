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
        private bool _Concluded;
        
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
                this._Concluded = true;

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

        public void Process()
        {
            if (this._ActionContainers.IsEmpty) return;

            List<string> cleanupIdList = new List<string>();
            
            // Prioritized the actions
            foreach (var (id, actionContainer) in this._ActionContainers)
            {
                if (actionContainer.Type != ActionType.Attached) continue;

                if (Factory.Available)
                {
                    this._AddHandler.Invoke(actionContainer);
                    continue;
                }
                
                actionContainer.Invoke();
                cleanupIdList.Add(id);
            }

            foreach (var (id, actionContainer) in this._ActionContainers)
            {
                if (actionContainer.Type != ActionType.External) continue;

                if (Factory.Available)
                {
                    this._AddHandler.Invoke(actionContainer);
                    continue;
                }
                
                actionContainer.Invoke();
                cleanupIdList.Add(id);
            }

            while (cleanupIdList.Count > 0)
            {
                this._ActionContainers.TryRemove(cleanupIdList[0], out _);
                cleanupIdList.RemoveAt(0);
            }
            if (this._ActionContainers.IsEmpty) return;
            
            Monitor.Enter(this._Lock);
            try
            {
                if (this._Concluded) return;
                    
                Monitor.Wait(this._Lock);
            }
            finally
            {
                Monitor.Exit(this._Lock);
            }
        }
    }
}
