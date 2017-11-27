using System;

namespace Xeora.Web.Basics
{
    public interface ILanguage : IDisposable
    {
        string ID { get; }
        string Name { get; }
        DomainInfo.LanguageInfo Info { get; }
        string Get(string TranslationID);
    }
}
