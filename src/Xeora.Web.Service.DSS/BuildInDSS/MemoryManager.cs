using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Dss
{
    public class MemoryManager : IDssManager
    {
        private object _ReservationLock;
        private readonly ConcurrentDictionary<string, Basics.Dss.IDss> _ReservationTable;

        private const short PRUNE_INTERVAL = 10 * 60; // 10 minutes
        private readonly System.Timers.Timer _PruneTimer;
        private readonly object _PruneLock;

        public MemoryManager()
        {
            this._ReservationLock = new object();
            this._ReservationTable = new ConcurrentDictionary<string, Basics.Dss.IDss>();

			this._PruneLock = new object();
            this._PruneTimer = new System.Timers.Timer(MemoryManager.PRUNE_INTERVAL * 1000);
            this._PruneTimer.Elapsed += this.PruneReservations;
            this._PruneTimer.Start();
        }

        public void Reserve(string uniqueId, int reservationTimeout, out Basics.Dss.IDss reservationObject)
        {
            lock (this._ReservationLock)
            {
                if (this.Get(uniqueId, out reservationObject))
                    return;

                this.Create(uniqueId, reservationTimeout, out reservationObject);
            }
        }

        private bool Get(string uniqueId, out Basics.Dss.IDss reservationObject)
        {
            reservationObject = null;

            if (string.IsNullOrEmpty(uniqueId))
                return false;

            if (!this._ReservationTable.TryGetValue(uniqueId, out reservationObject))
                return false;
                
            if (((IDssService)reservationObject).IsExpired)
            {
                if (!this._ReservationTable.TryRemove(uniqueId, out _))
                    throw new ReservationCreationException();

                reservationObject = null;

                return false;
            }

            ((IDssService)reservationObject).Extend();

            return true;
        }

        private void Create(string uniqueId, int reservationTimeout, out Basics.Dss.IDss reservationObject)
        {
            if (string.IsNullOrEmpty(uniqueId))
                throw new ReservationCreationException();

            reservationObject = new MemoryDss(uniqueId, reservationTimeout);

            if (!this._ReservationTable.TryAdd(uniqueId, reservationObject))
                throw new OutOfMemoryException();
        }

        private void PruneReservations(object sender, EventArgs args)
        {
            if (!Monitor.TryEnter(this._PruneLock))
                return;

            try
            {            
                string[] keys = new string[this._ReservationTable.Keys.Count];

                this._ReservationTable.Keys.CopyTo(keys, 0);

                foreach (string key in keys)
                {
                    this._ReservationTable.TryGetValue(key, out Basics.Dss.IDss reservation);

                    if (((IDssService)reservation).IsExpired)
                        this._ReservationTable.TryRemove(key, out reservation);
                }
			}
            finally
			{
                Monitor.Exit(this._PruneLock);
			}         
        }
    }
}
