using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application.Configurations
{
    public class Languages : ILanguages
    {
        private readonly ConcurrentDictionary<string, ILanguage> _Languages;
        private string _DefaultId;
        private ILanguage _Current;

        public Languages()
        {
            this._Languages = new ConcurrentDictionary<string, ILanguage>();
            this._DefaultId = string.Empty;
        }

        public void Add(ILanguage item)
        {
            if (item == null)
                return;

            this._Languages.TryAdd(item.Info.Id, item);
            if (item.Default)
                this._DefaultId = item.Info.Id;
        }

        public ILanguageDefinition this[string languageId] =>
            this._Languages.TryGetValue(languageId, out ILanguage language) ? language : null;

        public ILanguage Current => this._Current;

        public void Use(string languageId)
        {
            if (string.IsNullOrEmpty(languageId))
                languageId = this._DefaultId;

            this._Languages.TryGetValue(languageId, out this._Current);
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