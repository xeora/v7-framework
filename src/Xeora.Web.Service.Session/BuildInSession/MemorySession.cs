using System;
using System.Collections.Concurrent;

namespace Xeora.Web.Service.Session
{
    internal class MemorySession : Basics.Session.IHttpSession, IHttpSessionService
    {
        private ConcurrentDictionary<string, object> _Items;

        private int _ExpiresInMinute;

        public MemorySession(string sessionID, int expiresInMinutes)
        {
            this.SessionID = sessionID;
            this._ExpiresInMinute = expiresInMinutes;
            this._Items = new ConcurrentDictionary<string, object>();
            this.Extend();
        }

        public object this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                
                object value;
                if (this._Items.TryGetValue(key, out value))
                    return value;

                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                if (key.Length > 128)
                    throw new OverflowException("key can not be longer than 128 characters");

                this._Items.AddOrUpdate(key, value, (cKey, cValue) => value);
            }
        }

        public string SessionID { get; private set; }
        public DateTime Expires { get; private set; }

        public string[] Keys
        {
            get
            {
                string[] keys = new string[this._Items.Count];

                this._Items.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public bool IsExpired => DateTime.Compare(DateTime.Now, this.Expires) > 0;

        public void Extend() => 
            this.Expires = DateTime.Now.AddMinutes(this._ExpiresInMinute);
    }
}
