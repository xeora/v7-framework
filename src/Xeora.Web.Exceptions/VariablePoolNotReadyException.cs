namespace Xeora.Web.Exceptions
{
    public class VariablePoolNotReadyException : System.Exception
    {
        public VariablePoolNotReadyException() : 
            base("VariablePool is not ready yet!")
        { }
    }
}