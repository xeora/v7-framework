using System;

namespace Xeora.Web.Basics.Service
{
    public interface IVariablePool
    {
        DateTime LastAccess { get; }
        string SessionID { get; }

        void KeepAlive(string KeyID);

        byte[] Get(string keyID, string name);
        void Set(string keyID, string name, byte[] serializedValue);
        void Delete(string keyID);

        void Cleanup();
        void CopyInto(ref IVariablePool variablePool);
    }
}