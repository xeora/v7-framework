using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller
{
    public class ControllerSchedule
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _ScheduledItems;
        private readonly ControllerPool _Pool;

        public ControllerSchedule(ref ControllerPool pool)
        {
            this._ScheduledItems = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            this._Pool = pool;
        }

        public void Register(string boundControlID, string uniqueID)
        {
            if (!this._ScheduledItems.TryGetValue(boundControlID, out ConcurrentQueue<string> waitingIDs))
            {
                waitingIDs = new ConcurrentQueue<string>();

                if (!this._ScheduledItems.TryAdd(boundControlID, waitingIDs))
                {
                    this.Register(boundControlID, uniqueID);

                    return;
                }
            }

            waitingIDs.Enqueue(uniqueID);
        }

        public void Fire(string boundControlID)
        {
            if (!this._ScheduledItems.TryGetValue(boundControlID, out ConcurrentQueue<string> waitingIDs))
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
            this._Pool.GetInto(uniqueID, out IController controller);

            if (controller != null)
                controller.Render(uniqueID);
        }
    }
}
