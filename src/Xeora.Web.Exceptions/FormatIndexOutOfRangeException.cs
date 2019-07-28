namespace Xeora.Web.Exceptions
{
    public class FormatIndexOutOfRangeException : System.Exception
    {
        public FormatIndexOutOfRangeException() : 
            base("FormattableTranslation has un-matching index!")
        { }
    }
}
