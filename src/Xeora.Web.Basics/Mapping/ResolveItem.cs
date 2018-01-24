namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItem
    {
        public ResolveItem(string ID)
        {
            this.ID = ID;
            this.DefaultValue = string.Empty;
            this.QueryStringKey = ID;
        }

        public string ID { get; private set; }
        public string DefaultValue { get; set; }
        public string QueryStringKey { get; set; }
    }
}
