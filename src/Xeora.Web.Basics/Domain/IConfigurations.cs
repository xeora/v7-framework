namespace Xeora.Web.Basics.Domain
{
    public interface IConfigurations
    {
        string AuthenticationTemplate { get; }
        string DefaultTemplate { get; }
        string DefaultLanguage { get; }
        Enum.PageCachingTypes DefaultCaching { get; }
        string LanguageExecutable { get; }
        string SecurityExecutable { get; }
    }
}
