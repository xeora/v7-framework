namespace Xeora.Web.Manager
{
    public class StatementExecutable
    {
        public StatementExecutable(string executableName, string className, System.Exception exception)
        {
            this.ExecutableName = executableName;
            this.ClassName = className;
            this.Exception = exception;
        }

        public string ExecutableName { get; }
        public string ClassName { get; }
        public System.Exception Exception { get; }
    }
}
