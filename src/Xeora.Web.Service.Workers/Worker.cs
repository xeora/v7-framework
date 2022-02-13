using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    internal class Worker
    {
        internal const short THREAD_COUNT = 128;
        private const int JOIN_WAIT_TIMEOUT = 5000;
        
        private readonly BlockingCollection<ActionContainer> _Queue;
        private readonly Action<ActionType> _ActionNotifyHandler;
        
        private readonly ConcurrentStack<ActionContainer> _ExternalActionStack;
        private readonly Thread _Thread;
        private ActionContainer _CurrentContainer;

        public Worker(BlockingCollection<ActionContainer> queue, Action<ActionType> actionNotifyHandler)
        {
            this._Queue = queue;
            this._ActionNotifyHandler = actionNotifyHandler;
            
            this._ExternalActionStack = new ConcurrentStack<ActionContainer>();
            this._Thread =
                new Thread(this.Listen) {Priority = ThreadPriority.BelowNormal, IsBackground = true};
            this._Thread.Start();
        }

        private void Listen()
        {
            try
            {
                while (!this._Queue.IsAddingCompleted)
                {
                    if (!this._Queue.TryTake(out ActionContainer actionContainer))
                    {
                        if (this._ExternalActionStack.TryPop(out actionContainer))
                        {
                            this.Process(actionContainer);
                            continue;
                        }
                        actionContainer = this._Queue.Take();
                    }

                    if (actionContainer.Type == ActionType.External)
                    {
                        // We have an External to handle
                        this._ActionNotifyHandler.Invoke(ActionType.External);
                        this._ExternalActionStack.Push(actionContainer);
                        continue;
                    }
                    
                    this.Process(actionContainer);
                }
            }
            catch
            { /* just handle exception */ }
        }

        private void Process(ActionContainer actionContainer)
        {
            this._CurrentContainer = actionContainer;
            
            this.Processing = true;
            try
            {
                this._CurrentContainer.Invoke();
            }
            catch
            { /* Just handle exceptions */ }
            finally
            {
                this._CurrentContainer = null;
                this.Processing = false;
                
                // External is handled
                if (actionContainer.Type == ActionType.External)
                    this._ActionNotifyHandler.Invoke(ActionType.None);
            }
        }

        public void PrintReport() =>
            this._CurrentContainer?.PrintContainerDetails();

        public bool Processing { get; private set; }

        public void Join()
        {
            try
            {
                this._Thread.Join(Worker.JOIN_WAIT_TIMEOUT);
            }
            catch
            { /* Just handle exceptions */ }
        }
    }
}
