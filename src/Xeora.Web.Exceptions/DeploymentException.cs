namespace Xeora.Web.Exceptions
{
    public class DeploymentException : System.Exception
    {
        public DeploymentException(string message) : base(message)
        { }

        public DeploymentException(string message, System.Exception innerException) : base(message, innerException)
        { }
    }
}