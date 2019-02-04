using System;

namespace Xeora.Web.Site.Service
{
    [Serializable]
    public class VariablePool : Basics.Service.IVariablePool
    {
        private readonly Basics.DSS.IDSS _Reservation;

        public VariablePool(string sessionID, string keyID, ref Basics.DSS.IDSS reservation)
        {
            this.SessionID = sessionID;
            this.KeyID = keyID;

            this._Reservation = reservation;
        }

        public string SessionID { get; private set; }
        public string KeyID { get; private set; }

        public void Set(string name, byte[] serializedValue) =>
            this._Reservation[name] = serializedValue;

        public byte[] Get(string name) =>
            (byte[])this._Reservation[name];

        public void CopyInto(ref Basics.Service.IVariablePool variablePool)
        {
            foreach (string name in this._Reservation.Keys)
                variablePool.Set(name, this.Get(name));
        }
    }
}
