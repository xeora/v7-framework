using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service
{
    public class StatusTracker : Basics.IStatusTracker
    {
        private readonly ConcurrentDictionary<short, int> _Status;

        private StatusTracker() =>
            this._Status = new ConcurrentDictionary<short, int>();

        private static readonly object Lock = new object();
        private static StatusTracker _current;
        public static StatusTracker Current 
        { 
            get
            {
                Monitor.Enter(StatusTracker.Lock);
                try
                {
                    return StatusTracker._current ?? (StatusTracker._current = new StatusTracker());
                }
                finally
                {
                    Monitor.Exit(StatusTracker.Lock);
                }
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
