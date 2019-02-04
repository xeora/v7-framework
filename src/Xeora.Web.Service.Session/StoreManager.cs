using System;

namespace Xeora.Web.Service.Session
{
    public class StoreManager : IHttpSessionManager
    {
        private readonly short _ExpireInMinutes;

        public StoreManager(short expireInMinutes) =>
            this._ExpireInMinutes = expireInMinutes;

        public void Acquire(string sessionID, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            if (string.IsNullOrEmpty(sessionID))
            {
                sessionID = Guid.NewGuid().ToString();
                sessionID = sessionID.Replace("-", string.Empty);
                sessionID = sessionID.ToLowerInvariant();
            }

            DSS.DSSManager.Current.Reserve(sessionID, this._ExpireInMinutes, out Basics.DSS.IDSS reservation);

            if (reservation == null)
                throw new SessionCreationException();

            ReservationEnclosure enclosure = new ReservationEnclosure(sessionID, ref reservation);

            if (enclosure.IsExpired)
                return;

            enclosure.Extend();

            sessionObject = enclosure;
        }
    }
}
