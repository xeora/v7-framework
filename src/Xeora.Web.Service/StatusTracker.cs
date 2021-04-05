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

        public int GetRange(short min, short max)
        {
            int status = 0;
            
            foreach (short key in this._Status.Keys)
            {
                if (key > max && key < min) continue;
                status += this.Get(key);
            }

            return status;
        }
        
        public int Get1xx() =>
            this.GetRange(100, 199);
        
        public int Get2xx() =>
            this.GetRange(200, 299);
        
        public int Get3xx() =>
            this.GetRange(300, 399);
        
        public int Get4xx() =>
            this.GetRange(400, 499);

        public int Get5xx() =>
            this.GetRange(500, 599);
    }
}
