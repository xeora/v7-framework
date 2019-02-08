namespace Xeora.Web.Exception
{
    public class FormatIndexOutOfRangeException : System.Exception
    {
        public FormatIndexOutOfRangeException() : 
            base("FormattableTranslation has un-matching index!")
        { }
    }
}
