namespace Xeora.Web.Exceptions
{
    public class DomainNotExistsException : DeploymentException
    {
        public DomainNotExistsException(string message) : base(message)
        { }

        public DomainNotExistsException(string message, System.Exception innerException) : base(message, innerException)
        { }
    }
}