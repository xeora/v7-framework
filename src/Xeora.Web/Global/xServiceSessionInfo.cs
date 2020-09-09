using System;
using System.Collections;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    [Serializable]
    public class xServiceSessionInfo : IEnumerable
    {
        private readonly List<KeyValuePair<string, object>> _SessionItems;

        public xServiceSessionInfo(string publicKey, DateTime sessionDate)
        {
            this._SessionItems = new List<KeyValuePair<string, object>>();

            this.PublicKey = publicKey;
            this.SessionDate = sessionDate;
        }

        public string PublicKey { get; }
        public DateTime SessionDate { get; set; }

        public void AddSessionItem(string key, object value)
        {
            this.RemoveSessionItem(key);
            this._SessionItems.Add(
                new KeyValuePair<string, object>(key, value));
        }

        public void RemoveSessionItem(string key)
        {
            for (int iC = this._SessionItems.Count - 1; iC >= 0; iC += -1)
            {
                if (string.Compare(this._SessionItems[iC].Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                    this._SessionItems.RemoveAt(iC);
            }
        }

        public object this[string key]
        {
            get
            {
                foreach (var (itemKey, itemValue) in this._SessionItems)
                {
                    if (string.Compare(itemKey, key, StringComparison.OrdinalIgnoreCase) == 0)
                        return itemValue;
                }

                return null;
            }
        }

        public IEnumerator GetEnumerator() =>
            this._SessionItems.GetEnumerator();
    }
}