namespace Xeora.Web.Exceptions
{
    public class FormatIndexOutOfRangeException : System.Exception
    {
        public FormatIndexOutOfRangeException(string typeName) : 
            base($"{typeName} has un-matching index!")
        { }
    }
}
