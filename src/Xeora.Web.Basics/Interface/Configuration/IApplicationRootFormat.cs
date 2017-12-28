namespace Xeora.Web.Basics.Configuration
{
    public interface IApplicationRootFormat
    {
        string FileSystemImplementation { get; }
        string BrowserImplementation { get; }
    }
}
