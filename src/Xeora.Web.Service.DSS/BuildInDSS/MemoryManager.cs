using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.DSS
{
    public class MemoryManager : IDSSManager
    {
        private readonly ConcurrentDictionary<string, Basics.DSS.IDSS> _ReservationTable;

        private const short PRUNE_INTERVAL = 10 * 60; // 10 minutes
        private readonly System.Timers.Timer _PruneTimer;
        private readonly object _PruneLock;

        public MemoryManager()
        {
            this._ReservationTable = new ConcurrentDictionary<string, Basics.DSS.IDSS>();

			this._PruneLock = new object();
            this._PruneTimer = new System.Timers.Timer(MemoryManager.PRUNE_INTERVAL * 1000);
            this._PruneTimer.Elapsed += this.PruneReservations;
            this._PruneTimer.Start();
        }

        public void Reserve(string uniqueID, int reservationTimeout, out Basics.DSS.IDSS reservationObject)
        {
            if (this.Get(uniqueID, out reservationObject))
                return;

            this.Create(uniqueID, reservationTimeout, out reservationObject);
        }

        private bool Get(string uniqueID, out Basics.DSS.IDSS reservationObject)
        {
            reservationObject = null;

            if (string.IsNullOrEmpty(uniqueID))
                return false;

            if (!this._ReservationTable.TryGetValue(uniqueID, out reservationObject))
                return false;
                
            if (((IDSSService)reservationObject).IsExpired)
            {
                reservationObject = null;

                return false;
            }

            ((IDSSService)reservationObject).Extend();

            return true;
        }

        private void Create(string uniqueID, int reservationTimeout, out Basics.DSS.IDSS reservationObject)
        {
            if (string.IsNullOrEmpty(uniqueID))
                throw new ReservationCreationException();

            reservationObject = new MemoryDSS(uniqueID, reservationTimeout);

            if (!this._ReservationTable.TryAdd(uniqueID, reservationObject))
                throw new ReservationCreationException();
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
                    this._ReservationTable.TryGetValue(key, out Basics.DSS.IDSS reservation);

                    if (((IDSSService)reservation).IsExpired)
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
