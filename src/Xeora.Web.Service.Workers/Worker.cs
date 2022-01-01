using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Service.Workers
{
    internal class Worker
    {
        internal const short THREAD_COUNT = 8;

        private readonly short _Id;
        private readonly bool _Silent;
        private readonly BlockingCollection<ActionContainer> _Queue;
        private readonly List<WorkerThread> _Threads;
        private readonly object _AddLock = new object();

        public Worker(short id, short size, bool silent)
        {
            if (size < 1) size = Worker.THREAD_COUNT;
            
            this._Id = id;
            this._Silent = silent;
            this._Queue = 
                new BlockingCollection<ActionContainer>();
            this._Threads = new List<WorkerThread>();

            for (int i = 0; i < size; i++)
                this._Threads.Add(new WorkerThread(this._Queue));
            
            if (!this._Silent)
                Basics.Console.Push(string.Empty, $"Worker {this._Id} initialization is completed with {this._Threads.Count} thread!", string.Empty, false, true);
        }

        public int ThreadCount => this._Threads.Count;
        private long CountBusy()
        {
            long totalHasJob = 0;
            foreach (WorkerThread workerThread in this._Threads)
                totalHasJob += workerThread.Processing;

            return totalHasJob;
        }

        public void PrintReport()
        {
            string specificInfo = 
                this._Silent ? "Bucket " : string.Empty;
            
            if (this._Queue.IsAddingCompleted)
            {
                Basics.Console.Push($"{specificInfo}Worker Report",
                    $"{specificInfo}Worker {this._Id} has been already requested to be killed",
                    string.Empty, false, true);
                return;
            }

            long totalHasJob =
                this.CountBusy();
        
            Basics.Console.Push($"{specificInfo}Worker Report",
                $"{specificInfo}Worker {this._Id} has {this._Threads.Count} thread(s), running jobs: {totalHasJob}, waiting queue: {this._Queue.Count}",
                string.Empty, false, true);
            
            foreach (WorkerThread workerThread in this._Threads)
                workerThread.PrintThreadDetails();
        }

        public bool Promise(ActionContainer container)
        {
            if (this._Queue.IsAddingCompleted)
                return false;
            
            this._Queue.Add(container);
            return true;
        }

        public bool PromiseSafe(ActionContainer container)
        {
            lock (this._AddLock)
            {
                long available = 
                    this._Threads.Count - this.CountBusy();
                
                if (this._Queue.Count >= available)
                    return false;
            }

            return this.Promise(container);
        }

        public void Kill()
        {
            this._Queue.CompleteAdding();

            foreach (WorkerThread thread in this._Threads)
                thread.Join();
            
            if (!this._Silent)
                Basics.Console.Push(string.Empty, $"Worker {this._Id} is killed!", string.Empty, false, true);
        }
    }
}
