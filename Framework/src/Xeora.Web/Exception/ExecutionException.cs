namespace Xeora.Web.Exception
{
    public class ExecutionException : System.Exception
    {
        public ExecutionException() : 
            base("Execution failed!")
        { }

        public ExecutionException(string message) : 
            base(string.Format("Execution failed! - {0}", message))
        { }

        public ExecutionException(string message, System.Exception innerException) : 
            base(string.Format("Execution failed! - {0}", message), innerException)
        { }
    }
}