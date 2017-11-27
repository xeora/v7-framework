namespace Xeora.Web.Basics.Configuration
{
    public interface IUserSettings 
    {
        string this[string key] { get; }
    }
}
