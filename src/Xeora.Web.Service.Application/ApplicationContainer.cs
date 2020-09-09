using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Application
{
    public class ApplicationContainer : Basics.Application.IHttpApplication
    {
        private readonly ConcurrentDictionary<string, object> _Items;

        private ApplicationContainer() =>
            this._Items = new ConcurrentDictionary<string, object>();

        private static readonly object Lock = new object();
        private static Basics.Application.IHttpApplication _current;
        public static Basics.Application.IHttpApplication Current
        {
            get
            {
                Monitor.Enter(ApplicationContainer.Lock);
                try
                {
                    return ApplicationContainer._current ??
                           (ApplicationContainer._current = new ApplicationContainer());
                }
                finally
                {
                    Monitor.Exit(ApplicationContainer.Lock);
                }
            }
        }

        public object this[string key]
        {
            get => this._Items.TryGetValue(key, out object value) ? value : null;
            set => this._Items.AddOrUpdate(key, value, (cKey, cValue) => value);
        }

        public string[] Keys
        {
            get
            {
                string[] keys = 
                    new string[this._Items.Count];
                this._Items.Keys.CopyTo(keys, 0);

                return keys;
            }
        }
    }
}
