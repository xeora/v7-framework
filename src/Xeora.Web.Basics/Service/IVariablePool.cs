namespace Xeora.Web.Basics.Service
{
    public interface IVariablePool
    {
        string SessionID { get; }
        string KeyID { get; }

        byte[] Get(string name);
        void Set(string name, byte[] serializedValue);

        void CopyInto(ref IVariablePool variablePool);
    }
}