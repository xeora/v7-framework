namespace Xeora.Web.Basics.Domain
{
    public interface IServiceItem
    {
        string ID { get; set; }
        string MimeType { get; set; }
        bool Authentication { get; set; }
        string[] AuthenticationKeys { get; set; }
        bool StandAlone { get; set; }
        bool Overridable { get; set; }
        ServiceTypes ServiceType { get; set; }
        string ExecuteIn { get; set; }
    }
}
