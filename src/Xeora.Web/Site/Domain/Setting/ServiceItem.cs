namespace Xeora.Web.Site.Setting
{
    public class ServiceItem : Basics.Domain.IServiceItem
    {
        public ServiceItem(string Id)
        {
            this.Id = Id;
            this.MimeType = "text/html; charset=utf-8";
            this.ServiceType = Basics.Domain.ServiceTypes.Template;
            this.ExecuteIn = string.Empty;
            this.Authentication = false;
            this.AuthenticationKeys = new string[] { };
            this.StandAlone = false;
            this.Overridable = false;
        }

        public string Id { get; set; }
        public string MimeType { get; set; }
        public bool Authentication { get; set; }
        public string[] AuthenticationKeys { get; set; }
        public bool StandAlone { get; set; }
        public bool Overridable { get; set; }
        public Basics.Domain.ServiceTypes ServiceType { get; set; }
        public string ExecuteIn { get; set; }
    }
}
