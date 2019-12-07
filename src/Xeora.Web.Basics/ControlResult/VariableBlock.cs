using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class VariableBlock : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ConcurrentDictionary<string, object> _Items;

        public VariableBlock() =>
            this._Items = new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public Message Message { get; set; }

        public void Add(string key, object value) =>
            this._Items.AddOrUpdate(key, value, (k, v) => value);

        public object this[string key]
        {
            get => this._Items.TryGetValue(key, out object value) ? value : null;
            set => this.Add(key, value);
        }

        public IEnumerable<string> Keys
        {
            get
            {
                string[] keys = 
                    new string[this._Items.Keys.Count];
                this._Items.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
            this._Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
