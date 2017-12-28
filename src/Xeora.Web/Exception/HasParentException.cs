namespace Xeora.Web.Exception
{
    public class HasParentException : System.Exception
    {
        public HasParentException() : 
            base("Controller does not accept Parent!")
        { }
    }
}