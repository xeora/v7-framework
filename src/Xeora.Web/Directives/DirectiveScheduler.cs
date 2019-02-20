using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xeora.Web.Basics;

namespace Xeora.Web.Directives
{
    public class DirectiveScheduler
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _ScheduledItems;
        private readonly DirectivePool _Pool;

        public DirectiveScheduler(ref DirectivePool pool)
        {
            this._ScheduledItems = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            this._Pool = pool;
        }

        public void Register(string boundDirectiveID, string uniqueID)
        {
            if (!this._ScheduledItems.TryGetValue(boundDirectiveID, out ConcurrentQueue<string> waitingIDs))
            {
                waitingIDs = new ConcurrentQueue<string>();

                if (!this._ScheduledItems.TryAdd(boundDirectiveID, waitingIDs))
                {
                    this.Register(boundDirectiveID, uniqueID);

                    return;
                }
            }

            waitingIDs.Enqueue(uniqueID);
        }

        public void Fire(string boundDirectiveID)
        {
            if (!this._ScheduledItems.TryGetValue(boundDirectiveID, out ConcurrentQueue<string> waitingIDs))
                return;

            List<Task> runningJobs = new List<Task>();

            // To make function run on a different thread with the same settings
            // get the handlerID and assign it in the new thread.
            string handlerID = Helpers.CurrentHandlerID;

            while (!waitingIDs.IsEmpty)
            {
                waitingIDs.TryDequeue(out string uniqueID);

                runningJobs.Add(
                    Task.Run(() =>
                    {
                        Helpers.AssignHandlerID(handlerID);
                        this.RequestRender(uniqueID);
                    })
                );
            }

            foreach (Task task in runningJobs)
                task.Wait();
        }

        private void RequestRender(string uniqueID)
        {
            this._Pool.GetInto(uniqueID, out IDirective directive);

            if (directive != null)
                directive.Render(uniqueID);
        }
    }
}
