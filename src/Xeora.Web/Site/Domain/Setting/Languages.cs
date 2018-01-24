using System.Collections;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Site.Setting
{
    public class Languages : ILanguages
    {
        private Dictionary<string, ILanguage> _Languages;
        private string _DefaultID;

        public Languages()
        {
            this._Languages = new Dictionary<string, ILanguage>();
            this._DefaultID = string.Empty;
        }

        public void Add(ILanguage item)
        {
            if (item == null)
                return;

            this._Languages[item.Info.ID] = item;
            if (item.Default)
                this._DefaultID = item.Info.ID;
        }

        public ILanguageDefinition this[string languageID]
        {
            get
            {
                if (!this._Languages.ContainsKey(languageID))
                    return null;

                return this._Languages[languageID];
            }
        }

        public ILanguage Current { get; private set; }

        public void Use(string languageID)
        {
            if (string.IsNullOrEmpty(languageID))
                languageID = this._DefaultID;

            if (!this._Languages.ContainsKey(languageID))
            {
                this.Current = null;

                return;
            }
            
            this.Current = this._Languages[languageID];
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() =>
            this._Languages.Keys.GetEnumerator();

        public IEnumerator GetEnumerator() => 
            this._Languages.Keys.GetEnumerator();

        public void Dispose()
        {
            foreach (string languageID in this)
                this._Languages[languageID].Dispose();
        }
    }
}