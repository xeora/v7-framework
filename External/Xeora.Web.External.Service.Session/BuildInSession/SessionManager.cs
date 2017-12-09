using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.External.Service.Session
{
    public class SessionManager
    {
        private ConcurrentDictionary<string, Basics.Session.IHttpSession> _HttpSessionTable;

        private System.Timers.Timer _PruneTimer;
        private object _PruneLock;

        public SessionManager()
        {
            this._HttpSessionTable = new ConcurrentDictionary<string, Basics.Session.IHttpSession>();

            this._PruneLock = new object();
            this._PruneTimer = new System.Timers.Timer(60 * 1000); // Every Minute Run Pruning
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

        private string GetSessionKey(int remoteIP, string sessionID) =>
            string.Format("{0}-{1}", remoteIP, sessionID);

        public void Acquire(int remoteIP, string sessionID, short sessionTimeout, out Basics.Session.IHttpSession sessionObject)
        {
            string sessionKey = this.GetSessionKey(remoteIP, sessionID);

            if (this.Get(sessionKey, out sessionObject))
                return;

            this.Create(remoteIP, sessionTimeout, out sessionObject);
        }

        private bool Get(string sessionKey, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            if (string.IsNullOrEmpty(sessionKey))
                return false;

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

        private void Create(int remoteIP, short sessionTimeout, out Basics.Session.IHttpSession sessionObject)
        {
            string sessionID = Guid.NewGuid().ToString();
            sessionID = sessionID.Replace("-", string.Empty);
            sessionID = sessionID.ToLowerInvariant();

            string sessionKey = this.GetSessionKey(remoteIP, sessionID);

            sessionObject = new MemorySession(sessionID, sessionTimeout);

            if (!this._HttpSessionTable.TryAdd(sessionKey, sessionObject))
                throw new SessionCreationException();
        }

        private void PruneSessions(object sender, EventArgs args)
        {
            if (!Monitor.TryEnter(this._PruneLock))
                return;

            try
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
            finally 
            {
                Monitor.Exit(this._PruneLock);
            }
        }
    }
}
