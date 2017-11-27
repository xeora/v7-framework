using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller
{
    public class ControllerSchedule
    {
        private ConcurrentDictionary<string, ConcurrentQueue<string>> _ScheduledItems;
        private ControllerPool _Pool;

        public ControllerSchedule(ref ControllerPool pool)
        {
            this._ScheduledItems = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            this._Pool = pool;
        }

        public void Register(string boundControlID, string uniqueID)
        {
            ConcurrentQueue<string> waitingIDs = null;

            if (!this._ScheduledItems.TryGetValue(boundControlID, out waitingIDs))
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
            ConcurrentQueue<string> waitingIDs = null;

            if (!this._ScheduledItems.TryGetValue(boundControlID, out waitingIDs))
                return;

            List<Task> runningJobs = new List<Task>();

            // To make function run on a different thread with the same settings
            // get the handlerID and assign it in the new thread.
            string handlerID = Helpers.CurrentHandlerID;

            while (!waitingIDs.IsEmpty)
            {
                string uniqueID;
                waitingIDs.TryDequeue(out uniqueID);

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
            IController controller;
            this._Pool.GetInto(uniqueID, out controller);

            if (controller != null)
                controller.Render(uniqueID);
        }
    }
}
