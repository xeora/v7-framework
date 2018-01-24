using System;
using System.Collections;
using System.Collections.Generic;

namespace Xeora.Web.Basics.X
{
    public class SocketParameterCollection : IEnumerable
    {
        private Dictionary<string, object> _Parameters;

        internal SocketParameterCollection(KeyValuePair<string, object>[] parameters)
        {
            this._Parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    if (!this._Parameters.ContainsKey(item.Key))
                        this._Parameters.Add(item.Key, item.Value);
                }
            }
        }

        public object this[string key]
        {
            get
            {
                if (this._Parameters.ContainsKey(key))
                    return this._Parameters[key];

                return null;
            }
        }

        public IEnumerator GetEnumerator() =>
            this._Parameters.GetEnumerator();
    }
}
