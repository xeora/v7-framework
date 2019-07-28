using System.Threading;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Service;
using Xeora.Web.Application.Services;

namespace Xeora.Web.Handler
{
    public class RemoteInvoke
    {
        public static IHandler GetHandler(string handlerId) =>
            Manager.Current.Get(handlerId);

        public static IVariablePool GetVariablePool(string sessionId, string keyId)
        {
            PoolFactory.Get(sessionId, keyId, out IVariablePool variablePool);

            return variablePool;
        }

        private static readonly object ScheduledTaskEngineLock = new object();
        private static IScheduledTaskEngine _ScheduledTaskEngine;
        public static IScheduledTaskEngine GetScheduledTaskEngine()
        {
            Monitor.Enter(RemoteInvoke.ScheduledTaskEngineLock);
            try
            {
                if (RemoteInvoke._ScheduledTaskEngine == null)
                    RemoteInvoke._ScheduledTaskEngine = new ScheduledTasksEngine();
            }
            finally
            {
                Monitor.Exit(RemoteInvoke.ScheduledTaskEngineLock);
            }

            return RemoteInvoke._ScheduledTaskEngine;
        }

        public static void TransferVariablePool(string keyId, string fromSessionId, string toSessionId) =>
            PoolFactory.Copy(keyId, fromSessionId, toSessionId);

        public static Basics.Configuration.IXeora XeoraSettings => 
            Configuration.Manager.Current.Configuration;
    }
}
