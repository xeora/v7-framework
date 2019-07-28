using System.IO;
using System.Xml.XPath;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application.Configurations
{
    public class Language : ILanguage
    {
        private readonly StringReader _XPathStream;
        private readonly XPathNavigator _XPathNavigator;

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
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public bool Default { get; }
        public Basics.Domain.Info.Language Info { get; }

        public string Get(string translationId)
        {
            try
            {
                XPathNodeIterator xPathIter =
                    this._XPathNavigator.Select($"//translation[@id='{translationId}']");

                if (xPathIter.MoveNext())
                    return xPathIter.Current?.Value;

                throw new Exceptions.TranslationNotFoundException();
            }
            catch (Exceptions.TranslationNotFoundException)
            {
                throw;
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }

            return string.Empty;
        }

        public void Dispose() => _XPathStream?.Close();
    }
}