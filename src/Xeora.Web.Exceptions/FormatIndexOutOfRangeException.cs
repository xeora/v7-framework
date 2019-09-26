namespace Xeora.Web.Exceptions
{
    public class FormatIndexOutOfRangeException : System.Exception
    {
        public FormatIndexOutOfRangeException() : 
            base("ReplaceableTranslation has un-matching index!")
        { }
    }
}
