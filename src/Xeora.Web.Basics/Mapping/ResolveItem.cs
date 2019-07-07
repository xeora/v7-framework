namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItem
    {
        public ResolveItem(string Id)
        {
            this.Id = Id;
            this.DefaultValue = string.Empty;
            this.QueryStringKey = Id;
        }

        public string Id { get; private set; }
        public string DefaultValue { get; set; }
        public string QueryStringKey { get; set; }
    }
}
