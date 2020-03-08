using System;

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
            string lockCode = 
                this._Reservation.Lock(key);
        
            object value = 
                this._Reservation.Get(key);
            value = lockHandler?.Invoke(key, value);

            this._Reservation.Set(key, value, lockCode);
            this._Reservation.Release(key, lockCode);
            
            return value;
        }

        public T As<T>(string key)
        {
            object value =
                this[key];
            if (value == null) return default;

            return (T) Convert.ChangeType(value, typeof(T));
        }

        public string SessionId { get; }
        public DateTime Expires => this._Reservation.Expires;
        public string[] Keys => this._Reservation.Keys;

        public bool IsExpired => ((Dss.IService)this._Reservation).IsExpired;
    }
}
