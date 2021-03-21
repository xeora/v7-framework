using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Service.Dss.Internal
{
    public class Manager : IManager
    {
        private readonly object _ReservationLock;
        private readonly ConcurrentDictionary<string, Basics.Dss.IDss> _ReservationTable;

        private const short PRUNE_INTERVAL = 10 * 60; // 10 minutes
        private readonly object _PruneLock;

        public Manager()
        {
            this._ReservationLock = new object();
            this._ReservationTable = 
                new ConcurrentDictionary<string, Basics.Dss.IDss>();

			this._PruneLock = new object();
            
            System.Timers.Timer pruneTimer = 
                new System.Timers.Timer(Manager.PRUNE_INTERVAL * 1000);
            pruneTimer.Elapsed += this.PruneReservations;
            pruneTimer.Start();
        }

        public bool Reserve(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject)
        {
            lock (this._ReservationLock)
            {
                if (this.Get(uniqueId, out reservationObject))
                    return true;

                this.Create(uniqueId, reservationTimeout, out reservationObject);
            }

            return false;
        }

        private bool Get(string uniqueId, out Basics.Dss.IDss reservationObject)
        {
            reservationObject = null;

            if (string.IsNullOrEmpty(uniqueId))
                return false;

            if (!this._ReservationTable.TryGetValue(uniqueId, out reservationObject))
                return false;
                
            if (((IService)reservationObject).IsExpired)
            {
                this._ReservationTable.TryRemove(uniqueId, out _);
                
                reservationObject = null;
                return false;
            }

            ((IService)reservationObject).Extend();

            return true;
        }

        private void Create(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject)
        {
            if (string.IsNullOrEmpty(uniqueId))
                throw new Exceptions.ReservationCreationException();

            reservationObject = 
                new Service(uniqueId, reservationTimeout);
            this._ReservationTable.TryAdd(uniqueId, reservationObject);
        }

        private void PruneReservations(object sender, EventArgs args)
        {
            if (!Monitor.TryEnter(this._PruneLock))
                return;

            try
            {            
                string[] keys = 
                    new string[this._ReservationTable.Keys.Count];
                this._ReservationTable.Keys.CopyTo(keys, 0);

                foreach (string key in keys)
                {
                    if (!this._ReservationTable.TryGetValue(key, out Basics.Dss.IDss reservation))
                        continue;

                    if (((IService)reservation).IsExpired)
                        this._ReservationTable.TryRemove(key, out _);
                }
			}
            finally
			{
                Monitor.Exit(this._PruneLock);
			}         
        }
    }
}
