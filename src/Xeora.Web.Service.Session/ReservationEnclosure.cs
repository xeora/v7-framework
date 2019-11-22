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
            get => this._Reservation[key];
            set => this._Reservation[key] = value;
        }

        public string SessionId { get; }
        public DateTime Expires => this._Reservation.Expires;
        public string[] Keys => this._Reservation.Keys;

        public bool IsExpired => ((Dss.IService)this._Reservation).IsExpired;
    }
}
