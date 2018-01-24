namespace Xeora.Web.Exception
{
    public class LanguageFileException : System.Exception
    {
        public LanguageFileException() : 
            base("Language file has not found or has wrong format!")
        { }
    }
}