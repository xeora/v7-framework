using System;

namespace Xeora.Web.Site.Service
{
    internal class TaskInfo
    {
        private readonly Guid _ID;

        public TaskInfo(Action<object[]> callBack, object[] callBackParams, DateTime executionTime)
        {
            this._ID = Guid.NewGuid();

            this.CallBack = callBack;
            this.CallBackParams = callBackParams;

            this.ExecutionTime = executionTime;
        }

        public string ID => this._ID.ToString();
        public Action<object[]> CallBack { get; private set; }
        public object[] CallBackParams { get; private set; }
        public DateTime ExecutionTime { get; private set; }

        public void Execute() =>
            this.CallBack(this.CallBackParams);
    }
}
