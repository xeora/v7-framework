namespace Xeora.Web.Exceptions
{
    public class HasParentException : System.Exception
    {
        public HasParentException() : 
            base("Controller does not accept Parent!")
        { }
    }
}