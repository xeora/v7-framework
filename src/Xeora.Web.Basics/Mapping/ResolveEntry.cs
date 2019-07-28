namespace Xeora.Web.Basics.Mapping
{
    public class ResolveEntry
    {
        public ResolveEntry(ServiceDefinition serviceDefinition)
        {
            this.ServiceDefinition = serviceDefinition;
            this.MapFormat = string.Empty;
            this.ResolveItems = new ResolveItemCollection();
        }

        public ServiceDefinition ServiceDefinition { get; }
        public string MapFormat { get; set; }
        public ResolveItemCollection ResolveItems { get; }
    }
}
