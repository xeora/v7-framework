using System.Collections.Generic;

namespace Xeora.Web.Service.Context
{
    public class KeyValueCollection<K, V> : Basics.Context.IKeyValueCollection<K, V>
    {
        private Dictionary<K, V> _Container;

        public KeyValueCollection() =>
            this._Container = new Dictionary<K, V>();

        public KeyValueCollection(IEqualityComparer<K> comparer) =>
            this._Container = new Dictionary<K, V>(comparer);

        internal void AddOrUpdate(K key, V value) =>
            this._Container[key] = value;

        internal bool ContainsKey(K key) =>
            this._Container.ContainsKey(key);

        internal void Remove(K key)
        {
            if (this._Container.ContainsKey(key))
                this._Container.Remove(key);
        }

        public K[] Keys
        {
            get
            {
                K[] keys = new K[this._Container.Count];

                this._Container.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public V this[K key]
        {
            get
            {
                if (!this._Container.ContainsKey(key))
                    return default(V);

                return this._Container[key];
            }
        }
    }
}
