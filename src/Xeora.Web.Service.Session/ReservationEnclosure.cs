using System;

namespace Xeora.Web.Service.Session
{
    internal class ReservationEnclosure : Basics.Session.IHttpSession
    {
        private readonly Basics.DSS.IDSS _Reservation;

        public ReservationEnclosure(string sessionID, ref Basics.DSS.IDSS reservation)
        {
            this.SessionID = sessionID;
            this._Reservation = reservation;
        }

        public object this[string key]
        {
            get => this._Reservation[key];
            set => this._Reservation[key] = value;
        }

        public string SessionID { get; private set; }
        public DateTime Expires => this._Reservation.Expires;
        public string[] Keys => this._Reservation.Keys;

        public bool IsExpired => ((DSS.IDSSService)this._Reservation).IsExpired;
        public void Extend() => ((DSS.IDSSService)this._Reservation).Extend();
    }
}
