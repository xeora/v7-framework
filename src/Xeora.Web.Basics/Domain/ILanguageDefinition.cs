namespace Xeora.Web.Basics.Domain
{
    public interface ILanguageDefinition
    {
        bool Default { get; }
        Info.Language Info { get; }
    }
}
