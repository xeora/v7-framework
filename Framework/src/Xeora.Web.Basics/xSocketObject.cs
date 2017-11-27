using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    [Serializable()]
    public class xSocketObject
    {
        public xSocketObject(ref Context.IHttpContext context, KeyValuePair<string, object>[] parameters)
        {
            this.Request = context.Request;
            this.Response = context.Response;
            this.Parameters = new ParameterCollection(parameters);
        }

        public Context.IHttpRequest Request { get; private set; }
        public Context.IHttpResponse Response { get; private set; }
        public ParameterCollection Parameters { get; private set; }

        [Serializable()]
        public class ParameterCollection
        {
            private Dictionary<string, object> _Parameters;

            internal ParameterCollection(KeyValuePair<string, object>[] parameters)
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
        }
    }
}
