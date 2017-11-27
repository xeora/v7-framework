namespace Xeora.Web.Exception
{
    public class DirectDataAccessException : System.Exception
    {
        public DirectDataAccessException(System.Exception innerException) : 
            base("DirectDataAccess failed!", innerException)
        { }
    }
}