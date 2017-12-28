using Xeora.Web.Basics;
using Xeora.Web.Basics.Service;
using Xeora.Web.Site.Service;

namespace Xeora.Web.Handler
{
    public class RemoteInvoke
    {
        public static IHandler GetHandler(string handlerID) =>
            HandlerManager.Current.Get(handlerID);

        public static IVariablePool GetVariablePool(string sessionID)
        {
            IVariablePool rVP = null;

            PoolFactory.Get(sessionID, ref rVP);

            return rVP;
        }

        private static IScheduledTaskEngine _ScheduledTaskEngine = null;
        public static IScheduledTaskEngine GetScheduledTaskEngine()
        {
            if (RemoteInvoke._ScheduledTaskEngine == null)
                RemoteInvoke._ScheduledTaskEngine = new ScheduledTasksEngine();

            return RemoteInvoke._ScheduledTaskEngine;
        }

        public static void TransferVariablePool(string fromSessionID, string toSessionID) =>
            PoolFactory.Copy(fromSessionID, toSessionID);

        public static Basics.Configuration.IXeora XeoraSettings => 
            Configuration.ConfigurationManager.Current.Configuration;
    }
}
