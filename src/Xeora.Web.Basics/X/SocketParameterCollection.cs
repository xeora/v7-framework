using System;
using System.Collections;
using System.Collections.Generic;

namespace Xeora.Web.Basics.X
{
    public class SocketParameterCollection : IEnumerable
    {
        private readonly Dictionary<string, object> _Parameters;

        internal SocketParameterCollection(KeyValuePair<string, object>[] parameters)
        {
            this._Parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            if (parameters == null) return;
            
            foreach (KeyValuePair<string, object> item in parameters)
            {
                if (!this._Parameters.ContainsKey(item.Key))
                    this._Parameters.Add(item.Key, item.Value);
            }
        }

        public object this[string key] => 
            this._Parameters.ContainsKey(key) ? this._Parameters[key] : null;

        public IEnumerator GetEnumerator() =>
            this._Parameters.GetEnumerator();
    }
}
