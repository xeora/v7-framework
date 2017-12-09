namespace Xeora.Web.Service.Session
{
    public class SessionManager
    {
        private IHttpSessionManager _SessionManager;

        public SessionManager()
        {
            switch (Basics.Configurations.Xeora.Session.ServiceType)
            {
                case Basics.Configuration.SessionServiceTypes.External:
                    this._SessionManager = 
                        new ExternalManager(
                            Basics.Configurations.Xeora.Session.ServiceEndPoint, 
                            Basics.Configurations.Xeora.Session.Timeout
                        );

                    break;
                default:
                    this._SessionManager =
                        new MemoryManager(Basics.Configurations.Xeora.Session.Timeout);
                    
                    break;
            }
        }

        private static SessionManager _Current = null;
        public static IHttpSessionManager Current
        {
            get
            {
                if (SessionManager._Current == null)
                    SessionManager._Current = new SessionManager();

                return SessionManager._Current._SessionManager;
            }
        }
    }
}
