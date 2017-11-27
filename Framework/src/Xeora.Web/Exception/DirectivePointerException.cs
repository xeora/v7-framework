namespace Xeora.Web.Exception
{
    public class DirectivePointerException : System.Exception
    {
        public DirectivePointerException() : 
            base("Directive Pointer must be capital!")
        { }
    }
}