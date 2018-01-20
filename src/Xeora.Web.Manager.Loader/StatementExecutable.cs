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

        public string ExecutableName { get; private set; }
        public string ClassName { get; private set; }
        public System.Exception Exception { get; private set; }
    }
}
