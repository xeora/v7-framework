using System.Collections;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application.Domain.Configurations
{
    public class Languages : ILanguages
    {
        private readonly Dictionary<string, ILanguage> _Languages;
        private string _DefaultId;

        public Languages()
        {
            this._Languages = new Dictionary<string, ILanguage>();
            this._DefaultId = string.Empty;
        }

        public void Add(ILanguage item)
        {
            if (item == null)
                return;

            this._Languages[item.Info.Id] = item;
            if (item.Default)
                this._DefaultId = item.Info.Id;
        }

        public ILanguageDefinition this[string languageId] =>
            !this._Languages.ContainsKey(languageId) ? null : this._Languages[languageId];

        public ILanguage Current { get; private set; }

        public void Use(string languageId)
        {
            if (string.IsNullOrEmpty(languageId))
                languageId = this._DefaultId;

            if (!this._Languages.ContainsKey(languageId))
            {
                this.Current = null;

                return;
            }
            
            this.Current = this._Languages[languageId];
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() =>
            this._Languages.Keys.GetEnumerator();

        public IEnumerator GetEnumerator() => 
            this._Languages.Keys.GetEnumerator();

        public void Dispose()
        {
            foreach (string languageId in this)
                this._Languages[languageId].Dispose();
        }
    }
}