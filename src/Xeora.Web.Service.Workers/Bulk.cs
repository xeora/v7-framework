using System;
using System.Collections.Generic;

namespace Xeora.Web.Service.Workers
{
    public class Bulk
    {
        private readonly List<ActionContainer> _ActionContainers;
        private readonly Func<ActionContainer, bool> _AddHandler;

        internal Bulk(Func<ActionContainer, bool> addHandler)
        {
            this._ActionContainers = new List<ActionContainer>();
            this._AddHandler = addHandler;
        }

        public Task Add(Action<object> startHandler, object state)
        {
            ActionContainer actionContainer =
                new ActionContainer(startHandler, state);
            
            bool success = 
                this._AddHandler(actionContainer);
            if (!success) return null;

            this._ActionContainers.Add(actionContainer);
            return new Task(actionContainer);
        }

        public void Wait(Action<Exception> exceptionHandler = null)
        {
            foreach (ActionContainer actionContainer in this._ActionContainers)
            {
                Exception exception =
                    actionContainer.Wait();
                
                if (exception != null)
                    exceptionHandler?.Invoke(exception);
            }
        }
    }
}
