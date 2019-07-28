namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItem
    {
        public ResolveItem(string id)
        {
            this.Id = id;
            this.DefaultValue = string.Empty;
            this.QueryStringKey = id;
        }

        public string Id { get; }
        public string DefaultValue { get; set; }
        public string QueryStringKey { get; set; }
    }
}
