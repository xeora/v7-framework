namespace Xeora.Web.Basics.Configuration
{
    public interface IMain
    {
        string[] DefaultDomain { get; }
        string PhysicalRoot { get; }
        string VirtualRoot { get; }
        IApplicationRootFormat ApplicationRoot { get; }
        IWorkingPathFormat WorkingPath { get; }
        string TemporaryRoot { get; }
        bool Debugging { get; }
        bool Compression { get; }
        bool PrintAnalytics { get; }
        bool LogHTTPExceptions { get; }
        bool UseHTML5Header { get; }
        long Bandwidth { get; }
        string LoggingPath { get; }
    }
}
