using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Xeora.Web.Application.Services
{
    public class ScheduledTasksEngine : Basics.Service.IScheduledTaskEngine
    {
        private readonly ConcurrentDictionary<long, ConcurrentQueue<TaskInfo>> _ExecutionList;
        private readonly ConcurrentDictionary<string, bool> _ListOfCanceled;

        public ScheduledTasksEngine()
        {
            System.Timers.Timer executionTimer = 
                new System.Timers.Timer(1000);
            executionTimer.Elapsed += this.Execute;
            executionTimer.Start();

            this._ExecutionList = new ConcurrentDictionary<long, ConcurrentQueue<TaskInfo>>();
            this._ListOfCanceled = new ConcurrentDictionary<string, bool>();
        }

        public string RegisterTask(Action<object[]> scheduledCallBack, object[] @params, DateTime executionTime)
        {
            long executionId = Tools.DateTime.Format(executionTime);

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
            long executionId = Tools.DateTime.Format();

            if (!this._ExecutionList.TryRemove(executionId, out ConcurrentQueue<TaskInfo> queue)) return;
            
            ThreadPool.QueueUserWorkItem(this.ExecutionThread, queue);
        }

        private void ExecutionThread(object state)
        {
            ConcurrentQueue<TaskInfo> queue = (ConcurrentQueue<TaskInfo>)state;

            while (!queue.IsEmpty)
            {
                if (!queue.TryDequeue(out TaskInfo taskInfo)) continue;
                
                if (!this._ListOfCanceled.TryRemove(taskInfo.Id, out _))
                    ThreadPool.QueueUserWorkItem(
                        taskState =>
                        {
                            try
                            {
                                ((TaskInfo)taskState).Execute();
                            }
                            catch (Exception ex)
                            {
                                Tools.EventLogger.Log(ex);
                            }
                        },
                        taskInfo
                    );
            }
        }
    }
}
