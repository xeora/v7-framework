using System.Threading;

namespace Xeora.Web.Service.Session
{
    public class SessionManager
    {
        private readonly IHttpSessionManager _StoreManager;

        private SessionManager() =>
            this._StoreManager = new StoreManager(Basics.Configurations.Xeora.Session.Timeout);

        private static readonly object Lock = new object();
        private static SessionManager _current;
        public static IHttpSessionManager Current
        {
            get
            {
                Monitor.Enter(SessionManager.Lock);
                try
                {
                    SessionManager._current ??= new SessionManager();
                    return SessionManager._current._StoreManager;
                }
                finally
                {
                    Monitor.Exit(SessionManager.Lock);
                }
            }
        }
    }
}
