using System.Collections.Generic;

namespace Xeora.Web.Service.Context
{
    public class KeyValueCollection<TK, TV> : Basics.Context.IKeyValueCollection<TK, TV>
    {
        private readonly Dictionary<TK, TV> _Container;

        public KeyValueCollection() =>
            this._Container = new Dictionary<TK, TV>();

        public KeyValueCollection(IEqualityComparer<TK> comparer) =>
            this._Container = new Dictionary<TK, TV>(comparer);

        internal void AddOrUpdate(TK key, TV value) =>
            this._Container[key] = value;

        internal bool ContainsKey(TK key) =>
            this._Container.ContainsKey(key);

        internal void Remove(TK key)
        {
            if (this._Container.ContainsKey(key))
                this._Container.Remove(key);
        }

        public TK[] Keys
        {
            get
            {
                TK[] keys = new TK[this._Container.Count];

                this._Container.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public TV this[TK key] =>
            !this._Container.ContainsKey(key) ? default : this._Container[key];
    }
}
