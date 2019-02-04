using System;
using System.Collections;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Site.Setting;

namespace Xeora.Web.Site
{
    public class LanguagesHolder : ILanguages
    {
        private readonly ILanguages _Languages;
            
        public LanguagesHolder(IDomain owner, ILanguages languages)
        {
            this._Languages = new Languages();

            foreach (string languageID in languages)
            {
                ILanguage holder =
                    new LanguageHolder(owner, (ILanguage)languages[languageID]);

                ((Languages)this._Languages).Add(holder);
            }
        }

        public ILanguageDefinition this[string languageID] => this._Languages[languageID];
        public ILanguage Current => this._Languages.Current;

        public Action<ILanguage> LanguageChangedListener { get; set; }

        public void Use(string languageID)
        {
            this._Languages.Use(languageID);

            if (this._Languages.Current == null)
                throw new Exception.LanguageFileException();
            
            LanguageChangedListener?.Invoke(this._Languages.Current);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => this._Languages.GetEnumerator();
            
        public IEnumerator GetEnumerator() => this._Languages.GetEnumerator();

        public void Dispose() { /* Dispose will be handled by instance factory */ }
    }
}