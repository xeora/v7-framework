namespace Xeora.Web.Exceptions
{
    public class TranslationNotFoundException : System.Exception
    {
        public TranslationNotFoundException() : 
            base("Language file does not have the translation!")
        { }
    }
}