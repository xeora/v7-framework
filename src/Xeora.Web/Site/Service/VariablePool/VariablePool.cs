using System;

namespace Xeora.Web.Site.Service
{
    [Serializable]
    public class VariablePool : Basics.Service.IVariablePool
    {
        private readonly Basics.Dss.IDss _Reservation;

        public VariablePool(string sessionId, string keyId, ref Basics.Dss.IDss reservation)
        {
            this.SessionId = sessionId;
            this.KeyId = keyId;

            this._Reservation = reservation;
        }

        public string SessionId { get; private set; }
        public string KeyId { get; private set; }

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
