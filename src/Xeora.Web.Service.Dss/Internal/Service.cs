using System;
using System.Collections.Generic;

namespace Xeora.Web.Service.Dss.Internal
{
    internal class Service : Basics.Dss.IDss, IService
    {
        private readonly object _Lock = new object();
        private readonly Dictionary<string, ServiceItem> _Items;
        private readonly short _ExpiresInMinute;

        public Service(string uniqueId, short expiresInMinutes)
        {
            this.UniqueId = uniqueId;
            this._ExpiresInMinute = expiresInMinutes;
            this.Expires = DateTime.UtcNow.AddMinutes(this._ExpiresInMinute);
            this._Items = new Dictionary<string, ServiceItem>();
        }

        public string UniqueId { get; }
        public bool Reusing { get; private set; }
        public DateTime Expires { get; private set; }

        public string[] Keys
        {
            get
            {
                lock (this._Lock)
                {
                    string[] keys =
                        new string[this._Items.Count];
                    this._Items.Keys.CopyTo(keys, 0);

                    return keys;
                }
            }
        }
        
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            lock (this._Lock)
            {
                return this._Items.TryGetValue(key, out ServiceItem serviceItem) ? serviceItem.Get() : null;
            }
        }

        public void Set(string key, object value, string lockCode = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            if (key.Length > 128)
                throw new OverflowException("key can not be longer than 128 characters");

            ServiceItem serviceItem;
            lock (this._Lock)
            {
                if (!this._Items.TryGetValue(key, out serviceItem))
                {
                    serviceItem = 
                        new ServiceItem(key, value);
                    this._Items.Add(key, serviceItem);

                    return;
                }
            }
            serviceItem.Set(value, lockCode);
        }
        
        public string Lock(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            if (key.Length > 128)
                throw new OverflowException("key can not be longer than 128 characters");

            ServiceItem serviceItem;
            lock (this._Lock)
            {
                if (!this._Items.TryGetValue(key, out serviceItem))
                {
                    serviceItem = 
                        new ServiceItem(key);
                    this._Items.Add(key, serviceItem);
                }
            }
            return serviceItem.Lock();
        }

        public void Release(string key, string lockCode)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(lockCode))
                return;
            
            ServiceItem serviceItem;
            lock (this._Lock)
            {
                if (!this._Items.TryGetValue(key, out serviceItem))
                    return;
            }
            serviceItem.Release(lockCode);
        }

        public bool IsExpired => DateTime.Compare(DateTime.UtcNow, this.Expires) > 0;

        public void Extend()
        {
            this.Expires = DateTime.UtcNow.AddMinutes(this._ExpiresInMinute);
            this.Reusing = true;
        }
    }
}
