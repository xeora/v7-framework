using System.Collections.Concurrent;
using Xeora.Web.Basics.Service;
using Xeora.Web.Service.Dss;

namespace Xeora.Web.Site.Service
{
    public class PoolFactory : ConcurrentDictionary<string, IVariablePool>
    {
        private static PoolFactory _Current;
        private readonly short _ExpiresInMinutes ;

        private PoolFactory(short expiresInMinutes) =>
            this._ExpiresInMinutes = expiresInMinutes;

        public static void Initialize(short expiresInMinutes)
        {
            if (PoolFactory._Current != null)
                return;

            PoolFactory._Current = new PoolFactory(expiresInMinutes);
        }

        public static void Get(string sessionId, string keyId, out IVariablePool variablePool)
        {
            if (PoolFactory._Current == null)
                throw new Exception.VariablePoolNotReadyException();

            PoolFactory._Current.ProvideVariablePool(sessionId, keyId, out variablePool);
        }

        public static void Copy(string keyId, string fromSessionId, string toSessionId)
        {
            if (PoolFactory._Current == null)
                throw new Exception.VariablePoolNotReadyException();

            PoolFactory._Current.CopyVariablePool(keyId, fromSessionId, toSessionId);
        }

        private string CreatePoolKey(string sessionId, string keyId) =>
            string.Format("{0}_{1}", sessionId, keyId);

        private void ProvideVariablePool(string sessionId, string keyId, out IVariablePool variablePool)
        {
            string poolKey = this.CreatePoolKey(sessionId, keyId);

            DssManager.Current.Reserve(poolKey, this._ExpiresInMinutes, out Basics.Dss.IDss reservation);

            variablePool = new VariablePool(sessionId, keyId, ref reservation);
        }

        private void CopyVariablePool(string keyId, string fromSessionId, string toSessionId)
        {
            this.ProvideVariablePool(fromSessionId, keyId, out IVariablePool oldVariablePool);
            this.ProvideVariablePool(toSessionId, keyId, out IVariablePool newVariablePool);

            oldVariablePool.CopyInto(ref newVariablePool);
        }
    }
}
