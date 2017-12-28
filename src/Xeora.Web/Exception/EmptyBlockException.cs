namespace Xeora.Web.Exception
{
    public class EmptyBlockException : System.Exception
    {
        public EmptyBlockException() : 
            base("Empty Block is not allowed!")
        { }
    }
}
