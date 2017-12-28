namespace Xeora.Web.Exception
{
    public class InternalParentException : System.Exception
    {
        public enum ChildDirectiveTypes
        {
            Execution,
            Control
        }

        public InternalParentException(ChildDirectiveTypes childDirectiveType) : 
            base(string.Format("Parented {0} must not be located inside its parent!", childDirectiveType.ToString()))
        { }
    }
}