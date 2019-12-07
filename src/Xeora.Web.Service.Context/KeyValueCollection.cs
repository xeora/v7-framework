using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Service.Context
{
    public class KeyValueCollection<TK, TV> : Basics.Context.IKeyValueCollection<TK, TV>
    {
        private readonly ConcurrentDictionary<TK, TV> _Container;

        public KeyValueCollection() =>
            this._Container = new ConcurrentDictionary<TK, TV>();

        public KeyValueCollection(IEqualityComparer<TK> comparer) =>
            this._Container = new ConcurrentDictionary<TK, TV>(comparer);

        internal void AddOrUpdate(TK key, TV value) =>
            this._Container.AddOrUpdate(key, value, (k, v) => value);

        internal bool ContainsKey(TK key) =>
            this._Container.ContainsKey(key);

        internal void Remove(TK key) =>
            this._Container.TryRemove(key, out _);

        public TK[] Keys
        {
            get
            {
                TK[] keys = 
                    new TK[this._Container.Count];

                this._Container.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public TV this[TK key] =>
            !this._Container.TryGetValue(key, out TV value) ? default : value;
    }
}
