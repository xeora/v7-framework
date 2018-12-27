using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service
{
    public class StatusTracker : Basics.IStatusTracker
    {
        private ConcurrentDictionary<short, int> _Status;

        private StatusTracker() =>
            this._Status = new ConcurrentDictionary<short, int>();

        private static object _Lock = new object();
        private static StatusTracker _Current = null;
        public static StatusTracker Current 
        { 
            get
            {
                Monitor.Enter(StatusTracker._Lock);
                try
                {
                    if (StatusTracker._Current == null)
                        StatusTracker._Current = new StatusTracker();
                }
                finally
                {
                    Monitor.Exit(StatusTracker._Lock);
                }

                return StatusTracker._Current;
            }
        }

        internal void Increase(short statusCode) =>
            this._Status.AddOrUpdate(statusCode, 1, (k, v) => v + 1);

        public int Get(short statusCode)
        {
            this._Status.TryGetValue(statusCode, out int status);

            return status;
        }
    }
}
