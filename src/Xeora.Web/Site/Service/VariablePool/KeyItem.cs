using System;
using System.Collections.Concurrent;

namespace Xeora.Web.Site.Service
{
    public class KeyItem : ConcurrentDictionary<string, byte[]>
    {
        public KeyItem() =>
            this.LastAccess = DateTime.Now;

        public DateTime LastAccess { get; private set; }

        public void KeepAlive() =>
            this.LastAccess = DateTime.Now;

        public new bool TryGetValue(string key, out byte[] value)
        {
            this.LastAccess = DateTime.Now;

            return base.TryGetValue(key, out value);
        }

        public new bool TryAdd(string key, byte[] value)
        {
            this.LastAccess = DateTime.Now;

            base.AddOrUpdate(key, value, (itemKey, oldValue) => value);

            return true;
        }

        public KeyItem Clone()
        {
            KeyItem newKeyItem = new KeyItem();

            foreach (string key in base.Keys)
            {
                byte[] value;
                if (base.TryGetValue(key, out value))
                    newKeyItem.TryAdd(key, value);
            }

            return newKeyItem;
        }
    }
}
