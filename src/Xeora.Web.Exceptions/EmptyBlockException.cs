namespace Xeora.Web.Exceptions
{
    public class EmptyBlockException : System.Exception
    {
        public EmptyBlockException() : 
            base("Empty Block is not allowed!")
        { }
    }
}
