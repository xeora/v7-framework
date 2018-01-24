using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Site
{
    public class LanguageHolder : ILanguageDefinition, ILanguage
    {
        private IDomain _Owner;
        private ILanguage _Language;

        public LanguageHolder(IDomain owner, ILanguage language)
        {
            this._Owner = owner;
            this._Language = language;
        }

        public bool Default => this._Language.Default;
        public Basics.Domain.Info.Language Info => this._Language.Info;

        public string Get(string translationID)
        {
            try
            {
                return this._Language.Get(translationID);
            }
            catch (Exception.TranslationNotFoundException)
            {
                if (this._Owner.Parent != null)
                {
                    this._Owner.Parent.Languages.Use(this.Info.ID);

                    if (this._Owner.Parent.Languages.Current != null)
                        return this._Owner.Parent.Languages.Current.Get(translationID);
                }
            }

            return string.Empty;
        }

        public void Dispose() { /* Dispose will be handled by instance factory */ }
    }
}