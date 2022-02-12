using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    public class Factory
    {
        private readonly BlockingCollection<ActionContainer> _ActionQueue;
        private readonly List<Worker> _Workers;

        private Factory(int workerCount)
        {
            this._ActionQueue = 
                new BlockingCollection<ActionContainer>();

            this._Workers = new List<Worker>();

            if (workerCount < 1) workerCount = Worker.THREAD_COUNT;
            
            for (short i = 0; i < workerCount; i++)
                this._Workers.Add(new Worker(this._ActionQueue));

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

        public static Bulk CreateBulk() =>
            !Factory._current._ActionQueue.IsAddingCompleted
                ? new Bulk(a => Factory._current._ActionQueue.Add(a))
                : null;

        public static void Queue(Action<object> action, object state) =>
            Factory._current._ActionQueue.Add(new ActionContainer(action, state, ActionType.External));
        
        public static void Kill() =>
            Factory._current?._Kill();
    }
}
