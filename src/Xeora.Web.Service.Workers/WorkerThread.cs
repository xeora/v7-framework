using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    internal class WorkerThread
    {
        private const int JOIN_WAIT_TIMEOUT = 5000;
        
        private readonly BlockingCollection<ActionContainer> _Queue;
        private readonly Thread _Thread;
        private ActionContainer _CurrentContainer;
        private long _Processing;

        public WorkerThread(BlockingCollection<ActionContainer> queue)
        {
            this._Queue = queue;
            
            this._Thread =
                new Thread(this.Listen) {Priority = ThreadPriority.BelowNormal, IsBackground = true};
            this._Thread.Start();
        }
        
        private void Listen()
        {
            while (!this._Queue.IsAddingCompleted)
            {
                try
                {
                    this._CurrentContainer =
                        this._Queue.Take();
                    Interlocked.Increment(ref this._Processing);
                    
                    try
                    {
                        this._CurrentContainer.Invoke();
                        this._CurrentContainer = null;
                    }
                    catch
                    { /* Just handle exceptions */ }
                }
                catch
                {
                    break;
                }
                finally
                {
                    Interlocked.Decrement(ref this._Processing);
                }
            }
        }

        public void PrintThreadDetails()
        {
            ActionContainer container =
                this._CurrentContainer;

            container?.PrintContainerDetails();
        }

        public long Processing => 
            Interlocked.Read(ref this._Processing); 

        public void Join()
        {
            try
            {
                this._Thread.Join(WorkerThread.JOIN_WAIT_TIMEOUT);
            }
            catch
            { /* Just handle exceptions */ }
        }
    }
}
