using System;

namespace Xeora.Web.Basics.Service
{
    public interface ITaskSchedulerEngine
    {
        string RegisterTask(Action<object[]> schedulerCallBack, object[] @params, DateTime executionTime);
        string RegisterTask(Action<object[]> schedulerCallBack, object[] @params, TimeSpan executionTime);
        void UnRegisterTask(string id);
    }
}