using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Xml.XPath;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application.Configurations
{
    public class Language : ILanguage
    {
        private readonly object _Lock = new object();
        
        private readonly StringReader _XPathStream;
        private readonly XPathNavigator _XPathNavigator;

        private readonly ConcurrentDictionary<string, string> _Translations;

        internal Language(string xmlContent, bool @default)
        {
            if (xmlContent == null || xmlContent.Trim().Length == 0)
                throw new System.Exception(Global.SystemMessages.LANGUAGECONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(xmlContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--

                XPathNodeIterator xPathIter =
                    this._XPathNavigator.Select("/language");

                if (!xPathIter.MoveNext())
                    throw new Exceptions.LanguageFileException();

                this.Default = @default;
                this.Info =
                    new Basics.Domain.Info.Language(
                        xPathIter.Current?.GetAttribute("code", xPathIter.Current.NamespaceURI),
                        xPathIter.Current?.GetAttribute("name", xPathIter.Current.NamespaceURI)
                    );

                this._Translations = new ConcurrentDictionary<string, string>();
                this.Prepare();
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public bool Default { get; }
        public Basics.Domain.Info.Language Info { get; }

        private void Prepare()
        {
            XPathNodeIterator xPathIter =
                this._XPathNavigator.Select("//translation");

            while (xPathIter.MoveNext())
            {
                string id = 
                    xPathIter.Current?.GetAttribute("id", xPathIter.Current?.NamespaceURI);
                if (string.IsNullOrEmpty(id)) continue;
                
                this._Translations.TryAdd(id, xPathIter.Current?.Value);
            }
        }
        
        public string Get(string translationId)
        {
            if (this._Translations.TryGetValue(translationId, out string translation))
                return translation;
            
            Monitor.Enter(this._Lock);
            try
            {
                XPathNodeIterator xPathIter =
                    this._XPathNavigator.Select($"//translation[@id='{translationId}']");

                if (!xPathIter.MoveNext()) throw new Exceptions.TranslationNotFoundException();
                
                translation = 
                    xPathIter.Current?.Value;
                this._Translations.TryAdd(translationId, translation);
                
                return translation;
            }
            catch (Exceptions.TranslationNotFoundException)
            {
                throw;
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }
            finally
            {
                Monitor.Exit(this._Lock);
            }

            return string.Empty;
        }

        public void Dispose() => _XPathStream?.Close();
    }
}