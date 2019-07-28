namespace Xeora.Web.Exception
{
    public class ExecutionException : System.Exception
    {
        public ExecutionException() : 
            base("Execution failed!")
        { }

        public ExecutionException(string message) : 
            base($"Execution failed! - {message}")
        { }

        public ExecutionException(string message, System.Exception innerException) : 
            base($"Execution failed! - {message}", innerException)
        { }
    }
}