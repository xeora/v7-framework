using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Application
{
    public class ApplicationContainer : Basics.Application.IHttpApplication
    {
        private readonly ConcurrentDictionary<string, object> _Items;

        public ApplicationContainer() =>
            this._Items = new ConcurrentDictionary<string, object>();

        private static readonly object Lock = new object();
        private static Basics.Application.IHttpApplication _Current;
        public static Basics.Application.IHttpApplication Current
        {
            get
            {
                Monitor.Enter(ApplicationContainer.Lock);
                try
                {
                    return ApplicationContainer._Current ??
                           (ApplicationContainer._Current = new ApplicationContainer());
                }
                finally
                {
                    Monitor.Exit(ApplicationContainer.Lock);
                }
            }
        }

        public object this[string key]
        {
            get
            {
                if (this._Items.TryGetValue(key, out object value))
                    return value;

                return null;
            }
            set => this._Items.AddOrUpdate(key, value, (cKey, cValue) => value);
        }

        public string[] Keys
        {
            get
            {
                string[] keys = new string[this._Items.Count];

                this._Items.Keys.CopyTo(keys, 0);

                return keys;
            }
        }
    }
}
