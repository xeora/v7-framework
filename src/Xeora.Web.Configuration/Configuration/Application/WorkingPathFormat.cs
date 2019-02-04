namespace Xeora.Web.Configuration
{
    public class WorkingPathFormat : Basics.Configuration.IWorkingPathFormat
    {
        public WorkingPathFormat()
        {
            this.WorkingPath = string.Empty;
            this.WorkingPathID = string.Empty;
        }

        public string WorkingPath { get; internal set; }
        public string WorkingPathID { get; internal set; }
    }
}
