using System;

namespace Xeora.Web.Service.VariablePool
{
    [Serializable]
    public class PoolItem : Basics.Service.IVariablePool
    {
        private readonly Basics.Dss.IDss _Reservation;

        public PoolItem(string sessionId, string keyId, ref Basics.Dss.IDss reservation)
        {
            this.SessionId = sessionId;
            this.KeyId = keyId;

            this._Reservation = reservation;
        }

        public string SessionId { get; }
        public string KeyId { get; }

        public void Set(string name, byte[] serializedValue) =>
            this._Reservation.Set(name, serializedValue);

        public byte[] Get(string name) =>
            (byte[])this._Reservation.Get(name);

        public void CopyInto(ref Basics.Service.IVariablePool variablePool)
        {
            foreach (string name in this._Reservation.Keys)
                variablePool.Set(name, this.Get(name));
        }
    }
}
