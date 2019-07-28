namespace Xeora.Web.Exceptions
{
    public class InternalParentException : System.Exception
    {
        public enum ChildDirectiveTypes
        {
            Execution,
            Control
        }

        public InternalParentException(ChildDirectiveTypes childDirectiveType) : 
            base($"Parented {childDirectiveType.ToString()} must not be located inside its parent!")
        { }
    }
}