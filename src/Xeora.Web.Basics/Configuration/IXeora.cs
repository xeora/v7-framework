namespace Xeora.Web.Basics.Configuration
{
    public interface IXeora
    {
        IService Service { get; }
        IDSS DSS { get; }
        ISession Session { get; }
        IApplication Application { get; }
        IUserSettings User { get; }
    }
}
