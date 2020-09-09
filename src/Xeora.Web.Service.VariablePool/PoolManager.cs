using System.Collections.Concurrent;
using Xeora.Web.Basics.Service;

namespace Xeora.Web.Service.VariablePool
{
    public class PoolManager : ConcurrentDictionary<string, IVariablePool>
    {
        private static PoolManager _current;
        private readonly short _ExpiresInMinutes ;

        private PoolManager(short expiresInMinutes) =>
            this._ExpiresInMinutes = expiresInMinutes;

        public static void Initialize(short expiresInMinutes)
        {
            if (PoolManager._current != null)
                return;

            PoolManager._current = new PoolManager(expiresInMinutes);
        }

        public static void Get(string sessionId, string keyId, out IVariablePool variablePool)
        {
            if (PoolManager._current == null)
                throw new Exceptions.VariablePoolNotReadyException();

            PoolManager._current.ProvideVariablePool(sessionId, keyId, out variablePool);
        }

        public static void Copy(string keyId, string fromSessionId, string toSessionId)
        {
            if (PoolManager._current == null)
                throw new Exceptions.VariablePoolNotReadyException();

            PoolManager._current.CopyVariablePool(keyId, fromSessionId, toSessionId);
        }

        public static void KeepAlive(string sessionId, string keyId) =>
            PoolManager.Get(sessionId, keyId, out _);

        private static string CreatePoolKey(string sessionId, string keyId) => $"{sessionId}_{keyId}";

        private void ProvideVariablePool(string sessionId, string keyId, out IVariablePool variablePool)
        {
            string poolKey = PoolManager.CreatePoolKey(sessionId, keyId);

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
