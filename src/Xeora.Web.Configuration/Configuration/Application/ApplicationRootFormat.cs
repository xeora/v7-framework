namespace Xeora.Web.Configuration
{
    public class ApplicationRootFormat : Basics.Configuration.IApplicationRootFormat
    {
        public ApplicationRootFormat()
        {
            this.FileSystemImplementation = string.Empty;
            this.BrowserImplementation = string.Empty;
        }

        public string FileSystemImplementation { get; internal set; }

        public string BrowserImplementation { get; internal set; }
    }
}
