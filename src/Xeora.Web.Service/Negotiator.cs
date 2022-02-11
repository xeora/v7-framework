using System;
using System.Threading;
using Xeora.Web.Application;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Service;
using Xeora.Web.Service.TaskScheduler;
using Xeora.Web.Service.VariablePool;

namespace Xeora.Web.Service
{
    internal class Negotiator : INegotiator
    {
        private readonly object _TaskSchedulerEngineLock;
        private ITaskSchedulerEngine _TaskSchedulerEngine;
        
        public Negotiator() =>
            this._TaskSchedulerEngineLock = new object();

        public IHandler GetHandler(string handlerId) =>
            Handler.Manager.Current.Get(handlerId);
        
        public void KeepHandler(string handlerId) =>
            Handler.Manager.Current.Keep(handlerId);
        
        public void DropHandler(string handlerId) =>
            Handler.Manager.Current.Drop(handlerId);
        
        public IVariablePool GetVariablePool(string sessionId, string keyId)
        {
            PoolManager.Get(sessionId, keyId, out IVariablePool variablePool);
            return variablePool;
        }

        public IDomain CreateNewDomainInstance(string[] domainIdAccessTree, string domainLanguageId) =>
            (IDomain)Activator.CreateInstance(typeof(Domain), domainIdAccessTree, domainLanguageId);

        /*public void TransferVariablePool(string keyId, string fromSessionId, string toSessionId) =>
            PoolManager.Copy(keyId, fromSessionId, toSessionId);*/
        
        public IStatusTracker StatusTracker => Service.StatusTracker.Current;
        
        public ITaskSchedulerEngine TaskScheduler
        {
            get
            {
                Monitor.Enter(this._TaskSchedulerEngineLock);
                try
                {
                    return this._TaskSchedulerEngine ??= new SchedulerEngine();
                }
                finally
                {
                    Monitor.Exit(this._TaskSchedulerEngineLock);
                }
            }
        }
        
        public Basics.Configuration.IXeora XeoraSettings => 
            Configuration.Manager.Current.Configuration;
        
        public void ClearCache() =>
            Handler.Manager.Current.Refresh();
    }
}
