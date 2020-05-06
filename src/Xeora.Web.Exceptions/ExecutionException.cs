namespace Xeora.Web.Exceptions
{
    public class ExecutionException : System.Exception
    {
        public ExecutionException() : 
            base("Execution failed!")
        { }

        public ExecutionException(string message) : 
            base($"Execution failed! - {message}")
        { }

        public ExecutionException(System.Exception exception) : 
            base($"Execution failed! - {exception.Message}", exception)
        { }
    }
}