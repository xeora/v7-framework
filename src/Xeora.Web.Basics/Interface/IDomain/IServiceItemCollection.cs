namespace Xeora.Web.Basics
{
    public interface IServiceItemCollection
    {
        IServiceItem GetServiceItem(string ID);
        IServiceItemCollection GetServiceItems(ServiceTypes ServiceType);
        string[] GetAuthenticationKeys();
    }
}
