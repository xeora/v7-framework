using System.Threading;

namespace Xeora.Web.Service.Session
{
    public class SessionManager
    {
        private IHttpSessionManager _StoreManager;

        private SessionManager() =>
            this._StoreManager = new StoreManager(Basics.Configurations.Xeora.Session.Timeout);

        private static object _Lock = new object();
        private static SessionManager _Current = null;
        public static IHttpSessionManager Current
        {
            get
            {
                Monitor.Enter(SessionManager._Lock);
                try
                {
                    if (SessionManager._Current == null)
                        SessionManager._Current = new SessionManager();
                }
                finally
                {
                    Monitor.Exit(SessionManager._Lock);
                }

                return SessionManager._Current._StoreManager;
            }
        }
    }
}
