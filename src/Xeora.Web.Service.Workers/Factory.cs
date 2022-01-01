using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Xeora.Web.Basics.Configuration;

namespace Xeora.Web.Service.Workers
{
    public class Factory
    {
        private static IParallelism _parallelism;
        private readonly ConcurrentDictionary<short, Worker> _Workers;
        
        private readonly BlockingCollection<Worker> _Buckets;
        private readonly Dictionary<string, Bucket> _BucketRegistrations;
        private readonly object _BucketRegistrationLock = new object();
        private readonly BlockingCollection<ActionContainer> _ActionQueue;

        private Factory()
        {
            this._Workers = 
                new ConcurrentDictionary<short, Worker>();

            short worker = Factory._parallelism.Worker;
            if (worker < 1) worker = 1;
            
            for (short i = 0; i < worker; i++)
                this._Workers.TryAdd(i, new Worker(i, Factory._parallelism.WorkerThread, false));
            
            this._BucketRegistrations = new Dictionary<string, Bucket>();
            this._Buckets = new BlockingCollection<Worker>();

            int bucket = worker;
            if (!this._Workers.TryGetValue(0, out Worker workerObject))
                bucket *= Worker.THREAD_COUNT;
            else
                bucket *= workerObject.ThreadCount;

            int bucketCount = Factory._parallelism.BucketThread;
            for (short i = 0; i < bucket; i++)
            {
                Worker bucketWorker =
                    new Worker(i, Factory._parallelism.BucketThread, true);
                bucketCount = bucketWorker.ThreadCount;
                
                this._Buckets.Add(bucketWorker);
            }

            this._ActionQueue = new BlockingCollection<ActionContainer>();

            Thread factoryThread =
                new Thread(this.FactoryThread) {Priority = ThreadPriority.BelowNormal, IsBackground = true};
            factoryThread.Start();
            
            Basics.Console.Register(keyInfo => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.D)
                    return;

                foreach (KeyValuePair<short, Worker> pair in this._Workers)
                    pair.Value.PrintReport();

                foreach (KeyValuePair<string, Bucket> pair in this._BucketRegistrations)
                    pair.Value.PrintBucketDetails();
            });
            
            Basics.Console.Push(string.Empty, $"Worker Factory is running with {this._Workers.Count} Worker(s) on total {this._Workers.Count * this._Workers[0].ThreadCount} thread(s) and {this._Buckets.Count} Bucket(s) with possible {this._Buckets.Count * bucketCount} thread(s)", string.Empty, false, true);
        }

        private void FactoryThread()
        {
            short currentIndex = 0;
            
            while (!this._ActionQueue.IsAddingCompleted)
            {
                try
                {
                    ActionContainer container = 
                        this._ActionQueue.Take();
                    
                    if (this._Workers.TryGetValue(currentIndex, out Worker worker))
                        worker.Promise(container);
                    currentIndex = (short) ((currentIndex + 1) % this._Workers.Count);
                }
                catch
                {
                    Basics.Console.Push(string.Empty, "Worker Factory is killed!", string.Empty, false, true);
                    return;
                }
            }
        }
        
        private static readonly object Lock = new object();
        private static Factory _current;
        private static Factory CreateOrGet()
        {
            Monitor.Enter(Factory.Lock);
            try
            {
                if (Factory._parallelism == null)
                    return null;
                
                return Factory._current ?? 
                       (Factory._current = new Factory());
            }
            finally
            {
                Monitor.Exit(Factory.Lock);
            }
        }
        
        private Task _Queue(Action<object> startHandler, object state)
        {
            if (Factory._current == null)
                throw new Exception("Factory is not initialized");
            
            ActionContainer actionContainer = 
                new ActionContainer(startHandler, state);
            this._ActionQueue.Add(actionContainer);
            
            return new Task(actionContainer);
        }

        private Bucket _Bucket(string trackingId)
        {
            if (Factory._current == null)
                throw new Exception("Factory is not initialized");

            Monitor.Enter(this._BucketRegistrationLock);
            try
            {
                if (this._BucketRegistrations.ContainsKey(trackingId)) 
                    return this._BucketRegistrations[trackingId];

                Worker worker =
                    this._Buckets.Take();

                Bucket bucket = new Bucket(
                    trackingId,
                    a => worker.PromiseSafe(a),
                    () =>
                    {
                        Monitor.Enter(this._BucketRegistrationLock);
                        try
                        {
                            this._BucketRegistrations.Remove(trackingId);
                        }
                        finally
                        {
                            Monitor.Exit(this._BucketRegistrationLock);
                        }
                        
                        if (!this._Buckets.IsAddingCompleted)
                            this._Buckets.Add(worker);
                        else worker.Kill(); // If bucket is killed, kill the active worker
                    },
                    () => worker.PrintReport()
                );

                this._BucketRegistrations.Add(trackingId, bucket);

                return bucket;
            }
            catch
            {
                return null;
            }
            finally
            {
                Monitor.Exit(this._BucketRegistrationLock);
            }
        }
        
        private void _Kill()
        {
            if (Factory._current == null) return;
            
            Basics.Console.Push(string.Empty, "Worker Factory is draining...", string.Empty, false, true);
            
            this._Buckets.CompleteAdding();

            while (this._Buckets.Count > 0)
            {
                Worker worker = 
                    this._Buckets.Take();
                worker.Kill();
            }
            Basics.Console.Push(string.Empty, "Bucket Worker(s) are killed!", string.Empty, false, true);

            foreach (KeyValuePair<short, Worker> pair in this._Workers)
                pair.Value.Kill();
            
            Factory._current = null;
        }

        public static void Init(IParallelism parallelism)
        {
            Factory._parallelism = parallelism;
            Factory.CreateOrGet();
        }

        public static Task Queue(Action<object> startHandler, object state) =>
            Factory.CreateOrGet()?._Queue(startHandler, state);

        public static Bucket Bucket(string trackingId) =>
            Factory.CreateOrGet()?._Bucket(trackingId);

        public static void Kill() =>
            Factory.CreateOrGet()?._Kill();
    }
}
