namespace Xeora.Web.Exception
{
    public class RequestBlockException : System.Exception
    {
        public RequestBlockException() : 
            base("Request Block must not be placed inside in another Request Block")
        { }
    }
}