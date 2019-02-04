using System.Threading;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Service;
using Xeora.Web.Site.Service;

namespace Xeora.Web.Handler
{
    public class RemoteInvoke
    {
        public static IHandler GetHandler(string handlerID) =>
            HandlerManager.Current.Get(handlerID);

        public static IVariablePool GetVariablePool(string sessionID, string keyID)
        {
            PoolFactory.Get(sessionID, keyID, out IVariablePool rVP);

            return rVP;
        }

        private static object _ScheduledTaskEngineLock = new object();
        private static IScheduledTaskEngine _ScheduledTaskEngine = null;
        public static IScheduledTaskEngine GetScheduledTaskEngine()
        {
            Monitor.Enter(RemoteInvoke._ScheduledTaskEngineLock);
            try
            {
                if (RemoteInvoke._ScheduledTaskEngine == null)
                    RemoteInvoke._ScheduledTaskEngine = new ScheduledTasksEngine();
            }
            finally
            {
                Monitor.Exit(RemoteInvoke._ScheduledTaskEngineLock);
            }

            return RemoteInvoke._ScheduledTaskEngine;
        }

        public static void TransferVariablePool(string keyID, string fromSessionID, string toSessionID) =>
            PoolFactory.Copy(keyID, fromSessionID, toSessionID);

        public static Basics.Configuration.IXeora XeoraSettings => 
            Configuration.ConfigurationManager.Current.Configuration;
    }
}
