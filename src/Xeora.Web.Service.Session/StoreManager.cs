using System;

namespace Xeora.Web.Service.Session
{
    public class StoreManager : IHttpSessionManager
    {
        private readonly short _ExpireInMinutes;

        public StoreManager(short expireInMinutes) =>
            this._ExpireInMinutes = expireInMinutes;

        public void Acquire(string sessionId, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                sessionId = sessionId.Replace("-", string.Empty);
                sessionId = sessionId.ToLowerInvariant();
            }

            Dss.Manager.Current.Reserve(sessionId, this._ExpireInMinutes, out Basics.Dss.IDss reservation);

            if (reservation == null)
                throw new Exceptions.SessionCreationException();

            ReservationEnclosure enclosure = new ReservationEnclosure(sessionId, ref reservation);

            if (enclosure.IsExpired)
                return;

            enclosure.Extend();

            sessionObject = enclosure;
        }
    }
}
