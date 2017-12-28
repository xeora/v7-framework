using System;

namespace Xeora.Web.Basics.Service
{
    public interface IScheduledTaskEngine
    {
        string RegisterTask(Action<object[]> scheduledCallBack, object[] @params, DateTime executionTime);
        string RegisterTask(Action<object[]> scheduledCallBack, object[] @params, TimeSpan executionTime);
        void UnRegisterTask(string id);
    }
}