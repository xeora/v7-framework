namespace Xeora.Web.Exceptions
{
    public class UnknownDirectiveException : System.Exception
    {
        public UnknownDirectiveException() : 
            base("Directive pointer is not a known one!")
        { }
    }
}