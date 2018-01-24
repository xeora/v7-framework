using System;

namespace Xeora.Web.Basics.Domain
{
    public interface ILanguage : ILanguageDefinition, IDisposable
    {
        string Get(string translationID);
    }
}
