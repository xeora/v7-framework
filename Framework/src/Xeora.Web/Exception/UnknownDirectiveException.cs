namespace Xeora.Web.Exception
{
    public class UnknownDirectiveException : System.Exception
    {
        public UnknownDirectiveException() : 
            base("Directive pointer is not a known one!")
        { }
    }
}