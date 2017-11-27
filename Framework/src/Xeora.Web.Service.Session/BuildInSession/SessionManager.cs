using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Xeora.Web.Service.Session
{
    public class SessionManager
    {
        private int _SessionTimeout = 20;
        private ConcurrentDictionary<string, Basics.Session.IHttpSession> _HttpSessionTable;

        private Timer _PruneTimer;

        public SessionManager()
        {
            this._SessionTimeout = Basics.Configurations.Xeora.Session.Timeout;
            this._HttpSessionTable = new ConcurrentDictionary<string, Basics.Session.IHttpSession>();

            this._PruneTimer = new Timer(this._SessionTimeout * 1000);
            this._PruneTimer.Elapsed += this.PruneSessions;
            this._PruneTimer.Start();
        }

        private static SessionManager _Current = null;
        public static SessionManager Current
        {
            get
            {
                if (SessionManager._Current == null)
                    SessionManager._Current = new SessionManager();

                return SessionManager._Current;
            }
        }

        private string GetSessionKey(string remoteIP, string sessionID) =>
            string.Format("{0}-{1}", remoteIP, sessionID);

        public void Acquire(string remoteIP, string sessionID, out Basics.Session.IHttpSession sessionObject)
        {
            if (this.Get(remoteIP, sessionID, out sessionObject))
                return;

            this.Create(remoteIP, out sessionObject);
        }

        private bool Get(string remoteIP, string sessionID, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            if (string.IsNullOrEmpty(sessionID))
                return false;

            string sessionKey = this.GetSessionKey(remoteIP, sessionID);

            if (!this._HttpSessionTable.TryGetValue(sessionKey, out sessionObject))
                return false;

            if (((IHttpSessionService)sessionObject).IsExpired)
            {
                sessionObject = null;

                return false;
            }

            ((IHttpSessionService)sessionObject).Extend();

            return true;
        }

        private void Create(string remoteIP, out Basics.Session.IHttpSession sessionObject)
        {
            string sessionID = Guid.NewGuid().ToString();
            sessionID = sessionID.Replace("-", string.Empty);
            sessionID = sessionID.ToLowerInvariant();

            string sessionKey = this.GetSessionKey(remoteIP, sessionID);

            sessionObject = new MemorySession(sessionID, this._SessionTimeout);

            if (!this._HttpSessionTable.TryAdd(sessionKey, sessionObject))
                throw new SessionCreationException();
        }

        private void PruneSessions(object sender, EventArgs args)
        {
            string[] keys = new string[this._HttpSessionTable.Keys.Count];

            this._HttpSessionTable.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                Basics.Session.IHttpSession session;

                this._HttpSessionTable.TryGetValue(key, out session);

                if (((IHttpSessionService)session).IsExpired)
                    this._HttpSessionTable.TryRemove(key, out session);
            }
        }
    }
}
