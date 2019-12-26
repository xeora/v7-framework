using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Xeora.Web.Basics.Service;

namespace Xeora.Web.Service.VariablePool
{
    public class PoolManager : ConcurrentDictionary<string, IVariablePool>
    {
        private static PoolManager _Current;
        private readonly short _ExpiresInMinutes ;

        private PoolManager(short expiresInMinutes) =>
            this._ExpiresInMinutes = expiresInMinutes;

        public static void Initialize(short expiresInMinutes)
        {
            if (PoolManager._Current != null)
                return;

            PoolManager._Current = new PoolManager(expiresInMinutes);
        }

        public static void Get(string sessionId, string keyId, out IVariablePool variablePool)
        {
            if (PoolManager._Current == null)
                throw new Exceptions.VariablePoolNotReadyException();

            PoolManager._Current.ProvideVariablePool(sessionId, keyId, out variablePool);
        }

        public static void Copy(string keyId, string fromSessionId, string toSessionId)
        {
            if (PoolManager._Current == null)
                throw new Exceptions.VariablePoolNotReadyException();

            PoolManager._Current.CopyVariablePool(keyId, fromSessionId, toSessionId);
        }

        public static void KeepAlive(string sessionId, string keyId) =>
            PoolManager.Get(sessionId, keyId, out _);

        private string CreatePoolKey(string sessionId, string keyId) => $"{sessionId}_{keyId}";

        private void ProvideVariablePool(string sessionId, string keyId, out IVariablePool variablePool)
        {
            string poolKey = this.CreatePoolKey(sessionId, keyId);

            Dss.Manager.Current.Reserve(poolKey, this._ExpiresInMinutes, out Basics.Dss.IDss reservation);

            variablePool = new PoolItem(sessionId, keyId, ref reservation);
        }

        private void CopyVariablePool(string keyId, string fromSessionId, string toSessionId)
        {
            this.ProvideVariablePool(fromSessionId, keyId, out IVariablePool oldVariablePool);
            this.ProvideVariablePool(toSessionId, keyId, out IVariablePool newVariablePool);

            oldVariablePool.CopyInto(ref newVariablePool);
        }
    }
}
