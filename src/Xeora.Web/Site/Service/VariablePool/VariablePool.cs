using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Site.Service
{
    [Serializable]
    public class VariablePool : Basics.Service.IVariablePool
    {
        private ConcurrentDictionary<string, KeyItem> _KeyItems;

        private string _SessionID;
        private int _ExpiresInMinute;

        private DateTime _LastAccess;

        public VariablePool(string sessionID, int expiresInMinute)
        {
            this._KeyItems = new ConcurrentDictionary<string, KeyItem>();

            this._SessionID = sessionID;
            this._ExpiresInMinute = expiresInMinute;

            this._LastAccess = DateTime.Now;
        }

        public string SessionID => this._SessionID;
        public DateTime LastAccess => this._LastAccess;

        public void KeepAlive(string keyID)
        {
            KeyItem keyItem;
            if (this._KeyItems.TryGetValue(keyID, out keyItem))
                keyItem.KeepAlive();
        }

        public void Set(string keyID, string name, byte[] serializedValue)
        {
            this._LastAccess = DateTime.Now;

            KeyItem keyItem;

            if (!this._KeyItems.TryGetValue(keyID, out keyItem))
            {
                keyItem = new KeyItem();

                if (!this._KeyItems.TryAdd(keyID, keyItem))
                {
                    this.Set(keyID, name, serializedValue);

                    return;
                }
            }

            keyItem.TryAdd(name, serializedValue);
        }

        public byte[] Get(string keyID, string name)
        {
            this._LastAccess = DateTime.Now;

            KeyItem keyItem;
            if (!this._KeyItems.TryGetValue(keyID, out keyItem))
                return null;

            byte[] value;
            if (keyItem.TryGetValue(name, out value))
                return value;

            return null;
        }

        public void Delete(string keyID)
        {
            this._LastAccess = DateTime.Now;

            KeyItem dummy;
            this._KeyItems.TryRemove(keyID, out dummy);
        }

        public void Cleanup()
        {
            List<string> keysToDelete = new List<string>();

            foreach (string keyID in this._KeyItems.Keys)
            {
                KeyItem keyItem;
                if (this._KeyItems.TryGetValue(keyID, out keyItem))
                {
                    DateTime expireDate = keyItem.LastAccess.AddMinutes(this._ExpiresInMinute);

                    if (DateTime.Compare(DateTime.Now, expireDate) >= 0)
                        keysToDelete.Add(keyID);
                }
            }

            foreach (string deleteKeyID in keysToDelete)
            {
                KeyItem dummy;
                this._KeyItems.TryRemove(deleteKeyID, out dummy);
            }
        }

        public void CopyInto(ref Basics.Service.IVariablePool variablePool)
        {
            foreach (string keyID in this._KeyItems.Keys)
            {
                KeyItem keyItem;
                if (this._KeyItems.TryGetValue(keyID, out keyItem))
                    ((VariablePool)variablePool)._KeyItems.TryAdd(keyID, keyItem.Clone());
            }
        }
    }
}
