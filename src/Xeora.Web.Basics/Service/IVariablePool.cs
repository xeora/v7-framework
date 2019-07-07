namespace Xeora.Web.Basics.Service
{
    public interface IVariablePool
    {
        string SessionId { get; }
        string KeyId { get; }

        byte[] Get(string name);
        void Set(string name, byte[] serializedValue);

        void CopyInto(ref IVariablePool variablePool);
    }
}