using System;
using System.Collections.Concurrent;
using Xeora.Web.Exceptions;

namespace Xeora.Web.Service.Dss.Internal
{
    internal class Service : Basics.Dss.IDss, IService
    {
        private readonly ConcurrentDictionary<string, string> _Locks;
        private readonly ConcurrentDictionary<string, object> _Items;
        private readonly short _ExpiresInMinute;

        public Service(string uniqueId, short expiresInMinutes)
        {
            this.UniqueId = uniqueId;
            this._ExpiresInMinute = expiresInMinutes;
            this.Expires = DateTime.Now.AddMinutes(this._ExpiresInMinute);
            this._Locks = new ConcurrentDictionary<string, string>();
            this._Items = new ConcurrentDictionary<string, object>();
        }

        public string UniqueId { get; }
        public bool Reusing { get; private set; }
        public DateTime Expires { get; private set; }
        
        public object Get(string key, string lockCode = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            if (this._Locks.TryGetValue(key, out string code) && 
                string.CompareOrdinal(code, lockCode) != 0)
                throw new KeyLockedException();
                
            return this._Items.TryGetValue(key, out object value) ? value : null;
        }

        public void Set(string key, object value, string lockCode = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            if (key.Length > 128)
                throw new OverflowException("key can not be longer than 128 characters");

            if (this._Locks.TryGetValue(key, out string code) && 
                string.CompareOrdinal(code, lockCode) != 0)
                throw new KeyLockedException();
            
            this._Items.AddOrUpdate(key, value, (cKey, cValue) => value);
        }
        
        public string Lock(string key)
        {
            if (this._Locks.TryGetValue(key, out _))
                throw new KeyLockedException();

            string lockCode = 
                Guid.NewGuid().ToString();
            this._Locks.TryAdd(key, lockCode);

            return lockCode;
        }

        public void Release(string key, string lockCode)
        {
            if (this._Locks.TryGetValue(key, out string code) &&
                string.CompareOrdinal(code, lockCode) == 0)
                this._Locks.TryRemove(key, out _);
        }
        
        public string[] Keys
        {
            get
            {
                string[] keys = 
                    new string[this._Items.Count];
                this._Items.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public bool IsExpired => DateTime.Compare(DateTime.Now, this.Expires) > 0;

        public void Extend()
        {
            this.Expires = DateTime.Now.AddMinutes(this._ExpiresInMinute);
            this.Reusing = true;
        }
    }
}
