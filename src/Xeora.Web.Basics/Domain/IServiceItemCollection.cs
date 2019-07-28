namespace Xeora.Web.Basics.Domain
{
    public interface IServiceItemCollection
    {
        IServiceItem GetServiceItem(string id);
        IServiceItemCollection GetServiceItems(ServiceTypes serviceType);
        string[] GetAuthenticationKeys();
    }
}
