using System;
using System.Collections;
using System.Collections.Generic;
using Xeora.Web.Application.Configurations;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application
{
    public class LanguagesHolder : ILanguages
    {
        private readonly ILanguages _Languages;
            
        public LanguagesHolder(IDomain owner, ILanguages languages)
        {
            this._Languages = new Languages();

            foreach (string languageId in languages)
            {
                ILanguage holder =
                    new LanguageHolder(owner, (ILanguage)languages[languageId]);

                ((Languages)this._Languages).Add(holder);
            }
        }

        public ILanguageDefinition this[string languageId] => this._Languages[languageId];
        public ILanguage Current => this._Languages.Current;

        public Action<ILanguage> LanguageChangedListener { get; set; }

        public void Use(string languageId)
        {
            this._Languages.Use(languageId);
            
            if (this._Languages.Current == null)
                throw new Exceptions.LanguageFileException();
            
            LanguageChangedListener?.Invoke(this._Languages.Current);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => this._Languages.GetEnumerator();
            
        public IEnumerator GetEnumerator() => this._Languages.GetEnumerator();

        public void Dispose() { /* Dispose will be handled by instance factory */ }
    }
}