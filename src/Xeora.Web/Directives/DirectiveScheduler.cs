using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xeora.Web.Basics;

namespace Xeora.Web.Directives
{
    public class DirectiveScheduler
    {
        private readonly object _RegisterLock;

        private readonly ConcurrentDictionary<string, bool> _ScheduledItems;
        private readonly SemaphoreSlim _SemaphoreSlim;
        private readonly ConcurrentQueue<string> _Queue;

        private readonly Action<string> _CallBack;

        public DirectiveScheduler(Action<string> callback)
        {
            this._RegisterLock = new object();

            this._ScheduledItems = 
                new ConcurrentDictionary<string, bool>();
            this._SemaphoreSlim = 
                new SemaphoreSlim(Configurations.Xeora.Service.Parallelism);
            this._Queue = 
                new ConcurrentQueue<string>();

            this._CallBack = callback;
        }

        public void Register(string uniqueId)
        {
            lock (this._RegisterLock)
            {
                if (this._ScheduledItems.ContainsKey(uniqueId))
                    return;
                this._ScheduledItems.TryAdd(uniqueId, true);
                this._Queue.Enqueue(uniqueId);
            }
        }

        public void Fire()
        {
            string handlerId = Helpers.CurrentHandlerId;
            List<Task> callbackJobs = new List<Task>();

            while (this._Queue.TryDequeue(out string uniqueId))
            {
                this._ScheduledItems.TryRemove(uniqueId, out _);
                
                callbackJobs.Add(
                    Task.Factory.StartNew(uId =>
                    {
                        Helpers.AssignHandlerId(handlerId);

                        this._SemaphoreSlim.Wait();
                        try
                        {
                            this._CallBack((string)uId);
                        }
                        catch (Exception ex)
                        {
                            Basics.Console.Push("Execution Exception...", ex.Message, ex.StackTrace, false, true, type: Basics.Console.Type.Error);
                        }
                        finally
                        {
                            this._SemaphoreSlim.Release();
                        }
                    }, uniqueId)
                );
            }

            try
            {
                if (callbackJobs.Count > 0)
                    Task.WaitAll(callbackJobs.ToArray());
            }
            catch (Exception ex)
            {
                Basics.Console.Push("Execution Exception...", ex.Message, ex.StackTrace, false, true, type: Basics.Console.Type.Error);
            }
        }
    }
}
