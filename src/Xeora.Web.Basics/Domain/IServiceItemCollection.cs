namespace Xeora.Web.Basics.Domain
{
    public interface IServiceItemCollection
    {
        IServiceItem GetServiceItem(string Id);
        IServiceItemCollection GetServiceItems(ServiceTypes ServiceType);
        string[] GetAuthenticationKeys();
    }
}
