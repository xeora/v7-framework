namespace Xeora.Web.Basics
{
    public interface IConfigurations
    {
        string AuthenticationPage { get; }
        string DefaultPage { get; }
        string DefaultLanguage { get; }
        Enum.PageCachingTypes DefaultCaching { get; }
        string DefaultSecurityBind { get; }
    }
}
