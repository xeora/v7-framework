namespace Xeora.Web.Basics.Domain
{
    public interface IServiceItemCollection
    {
        IServiceItem GetServiceItem(string ID);
        IServiceItemCollection GetServiceItems(ServiceTypes ServiceType);
        string[] GetAuthenticationKeys();
    }
}
