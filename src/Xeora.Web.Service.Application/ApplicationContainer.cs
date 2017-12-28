using System.Collections.Concurrent;

namespace Xeora.Web.Service.Application
{
    public class ApplicationContainer : Basics.Application.IHttpApplication
    {
        private ConcurrentDictionary<string, object> _Items;

        public ApplicationContainer() =>
            this._Items = new ConcurrentDictionary<string, object>();

        private static Basics.Application.IHttpApplication _Current = null;
        public static Basics.Application.IHttpApplication Current
        {
            get
            {
                if (ApplicationContainer._Current == null)
                    ApplicationContainer._Current = new ApplicationContainer();

                return ApplicationContainer._Current;
            }
        }

        public object this[string key]
        {
            get
            {
                object value;
                if (this._Items.TryGetValue(key, out value))
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
