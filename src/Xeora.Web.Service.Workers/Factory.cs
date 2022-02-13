using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

// The worker control is developed to skip the bottleneck of .NET async Task operation.
// Async task operations are based on scheduling but they are too slow for web requests
// and it ends with slow response time and latency on service. That's why, "Workers"
// implementation is providing agile and more responsive web service for Xeora.
namespace Xeora.Web.Service.Workers
{
    public class Factory
    {
        private readonly BlockingCollection<ActionContainer> _ActionQueue;
        private readonly List<Worker> _Workers;

        private readonly int _WorkerCount;
        private long _ExternalLoad;

        private Factory(int workerCount)
        {
            this._WorkerCount = workerCount;
            this._ActionQueue = 
                new BlockingCollection<ActionContainer>();

            this._Workers = new List<Worker>();

            if (workerCount < 1) workerCount = Worker.THREAD_COUNT;
            
            for (short i = 0; i < workerCount; i++)
                this._Workers.Add(new 
                    Worker(
                        this._ActionQueue,
                        actionType =>
                        {
                            switch (actionType)
                            {
                                case ActionType.External:
                                    Interlocked.Increment(ref this._ExternalLoad);
                                    break;
                                case ActionType.None:
                                    Interlocked.Decrement(ref this._ExternalLoad);
                                    break;
                            }
                        }
                    )
                );

            Basics.Console.Register(keyInfo => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.D)
                    return;

                if (this._ActionQueue.IsAddingCompleted)
                {
                    Basics.Console.Push(
                        string.Empty, 
                        "Worker Factory has been already requested to be killed",
                        string.Empty, false, true);
                    return;
                }

                short processing = 0;
                foreach (Worker workerThread in this._Workers)
                {
                    if (!workerThread.Processing) continue;

                    processing++;
                    workerThread.PrintReport();
                }

                Basics.Console.Push(string.Empty, $"Worker Factory is processing {processing} Task(s), {this._Workers.Count - processing} available Worker(s) in total {this._Workers.Count} Worker(s)", string.Empty, false, true);    
            });
        }

        private bool Full => 
            Interlocked.Read(ref this._ExternalLoad) >= this._WorkerCount;
        
        private void _Kill()
        {
            if (Factory._current == null) return;
            
            Basics.Console.Push(string.Empty, "Worker Factory is draining...", string.Empty, false, true);

            this._ActionQueue.CompleteAdding();
            this._ActionQueue.Dispose();
            
            foreach (Worker worker in this._Workers)
                worker.Join();
            
            Factory._current = null;
            
            Basics.Console.Push(string.Empty, "Worker Factory is killed!", string.Empty, false, true);
        }

        private static readonly object Lock = new object();
        private static Factory _current;

        public static void Init(int workerCount)
        {
            Monitor.Enter(Factory.Lock);
            try
            {
                Factory._current ??= new Factory(workerCount);
            }
            finally
            {
                Monitor.Exit(Factory.Lock);
            }
        }
        
        public static Bulk CreateBulk() =>
            !Factory._current._ActionQueue.IsAddingCompleted
                ? new Bulk(a => Factory._current._ActionQueue.Add(a))
                : null;

        public static void Queue(Action<object> action, object state) =>
            Factory._current._ActionQueue.Add(new ActionContainer(action, state, ActionType.External));

        public static bool Available => !Factory._current.Full;
        
        public static void Kill() =>
            Factory._current?._Kill();
    }
}
