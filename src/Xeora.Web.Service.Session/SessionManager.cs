using System.Threading;

namespace Xeora.Web.Service.Session
{
    public class SessionManager
    {
        private readonly IHttpSessionManager _StoreManager;

        private SessionManager() =>
            this._StoreManager = new StoreManager(Basics.Configurations.Xeora.Session.Timeout);

        private static readonly object Lock = new object();
        private static SessionManager _Current;
        public static IHttpSessionManager Current
        {
            get
            {
                Monitor.Enter(SessionManager.Lock);
                try
                {
                    if (SessionManager._Current == null)
                        SessionManager._Current = new SessionManager();
                }
                finally
                {
                    Monitor.Exit(SessionManager.Lock);
                }

                return SessionManager._Current._StoreManager;
            }
        }
    }
}
