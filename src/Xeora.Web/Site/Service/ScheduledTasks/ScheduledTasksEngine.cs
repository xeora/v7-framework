using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Xeora.Web.Site.Service
{
    public class ScheduledTasksEngine : Basics.Service.IScheduledTaskEngine
    {
        private readonly System.Timers.Timer _ExecutionTimer;
        private readonly ConcurrentDictionary<long, ConcurrentQueue<TaskInfo>> _ExecutionList;
        private readonly ConcurrentDictionary<string, bool> _ListOfCanceled;

        public ScheduledTasksEngine()
        {
            this._ExecutionTimer = new System.Timers.Timer(1000);
            this._ExecutionTimer.Elapsed += this.Execute;
            this._ExecutionTimer.Start();

            this._ExecutionList = new ConcurrentDictionary<long, ConcurrentQueue<TaskInfo>>();
            this._ListOfCanceled = new ConcurrentDictionary<string, bool>();
        }

        public string RegisterTask(Action<object[]> scheduledCallBack, object[] @params, DateTime executionTime)
        {
            long executionId = Helper.DateTime.Format(executionTime);

            if (!this._ExecutionList.TryGetValue(executionId, out ConcurrentQueue<TaskInfo> queue))
            {
                queue = new ConcurrentQueue<TaskInfo>();

                if (!this._ExecutionList.TryAdd(executionId, queue))
                    return this.RegisterTask(scheduledCallBack, @params, executionTime);
            }

            TaskInfo taskInfo = new TaskInfo(scheduledCallBack, @params, executionTime);

            queue.Enqueue(taskInfo);

            return taskInfo.Id;
        }

        public string RegisterTask(Action<object[]> scheduledCallBack, object[] @params, TimeSpan executionTime)
        {
            DateTime absoluteExecutionTime = DateTime.Now.Add(executionTime);

            return this.RegisterTask(scheduledCallBack, @params, absoluteExecutionTime);
        }

        public void UnRegisterTask(string id) =>
            this._ListOfCanceled.TryAdd(id, true);

        private void Execute(object sender, EventArgs args)
        {
            long executionId = Helper.DateTime.Format();

            if (this._ExecutionList.TryRemove(executionId, out ConcurrentQueue<TaskInfo> queue))
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecutionThread), queue);
        }

        private void ExecutionThread(object state)
        {
            ConcurrentQueue<TaskInfo> queue = (ConcurrentQueue<TaskInfo>)state;

            while (!queue.IsEmpty)
            {
                if (queue.TryDequeue(out TaskInfo taskInfo))
                {
                    if (!this._ListOfCanceled.TryRemove(taskInfo.Id, out bool dummy))
                        ThreadPool.QueueUserWorkItem(
                            (taskState) =>
                            {
                                try
                                {
                                    ((TaskInfo)taskState).Execute();
                                }
                                catch (System.Exception ex)
                                {
                                    Helper.EventLogger.Log(ex);
                                }
                            },
                            taskInfo
                        );
                }
            }
        }
    }
}
