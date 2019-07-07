namespace Xeora.Web.Configuration
{
    public class WorkingPathFormat : Basics.Configuration.IWorkingPathFormat
    {
        public WorkingPathFormat()
        {
            this.WorkingPath = string.Empty;
            this.WorkingPathId = string.Empty;
        }

        public string WorkingPath { get; internal set; }
        public string WorkingPathId { get; internal set; }
    }
}
