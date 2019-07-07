namespace Xeora.Web.Basics.Configuration
{
    public interface IXeora
    {
        IService Service { get; }
        IDss Dss { get; }
        ISession Session { get; }
        IApplication Application { get; }
        IUserSettings User { get; }
    }
}
