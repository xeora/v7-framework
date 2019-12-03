using System;
using System.Threading;
using Xeora.Web.Exceptions;

namespace Xeora.Web.Service.Session
{
    internal class ReservationEnclosure : Basics.Session.IHttpSession
    {
        private readonly Basics.Dss.IDss _Reservation;

        public ReservationEnclosure(string sessionId, ref Basics.Dss.IDss reservation)
        {
            this.SessionId = sessionId;
            this._Reservation = reservation;
        }

        public object this[string key]
        {
            get => this._Reservation.Get(key);
            set => this._Reservation.Set(key, value);
        }

        public object Lock(string key, Func<string, object, object> lockHandler)
        {
            do
            {
                string lockCode = string.Empty;
                try
                {
                    lockCode = this._Reservation.Lock(key);
                    if (string.IsNullOrEmpty(lockCode))
                        throw new ArgumentException();

                    object value =
                        this._Reservation.Get(key, lockCode);
                    value = lockHandler?.Invoke(key, value);
                    
                    this._Reservation.Set(key, value, lockCode);

                    return value;
                }
                catch (KeyLockedException)
                {
                    Thread.Sleep(1);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(lockCode))
                        this._Reservation.Release(key, lockCode);
                }
            } while (true);
        }

        public string SessionId { get; }
        public DateTime Expires => this._Reservation.Expires;
        public string[] Keys => this._Reservation.Keys;

        public bool IsExpired => ((Dss.IService)this._Reservation).IsExpired;
    }
}
