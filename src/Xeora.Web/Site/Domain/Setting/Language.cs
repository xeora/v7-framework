using System;
using System.IO;
using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Language : Basics.ILanguage
    {
        private StringReader _XPathStream = null;
        private XPathNavigator _XPathNavigator;

        public Language(string languageXMLContent)
        {
            if (languageXMLContent == null || languageXMLContent.Trim().Length == 0)
                throw new System.Exception(Global.SystemMessages.TRANSLATIONCONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(languageXMLContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--

                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select("/language");

                if (xPathIter.MoveNext())
                {
                    this.ID = xPathIter.Current.GetAttribute("code", xPathIter.Current.NamespaceURI);
                    this.Name = xPathIter.Current.GetAttribute("name", xPathIter.Current.NamespaceURI);
                }
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public string ID { get; private set; }
        public string Name { get; private set; }
        public Basics.DomainInfo.LanguageInfo Info => 
            new Basics.DomainInfo.LanguageInfo(this.ID, this.Name);

        public string Get(string translationID)
        {
            try
            {
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select(string.Format("//translation[@id='{0}']", translationID));

                if (xPathIter.MoveNext())
                    return xPathIter.Current.Value;

                throw new Exception.TranslationNotFoundException();
            }
            catch (Exception.TranslationNotFoundException)
            {
                throw;
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }

            return string.Empty;
        }

        public void Dispose()
        {
            if (this._XPathStream != null)
            {
                this._XPathStream.Close();
                GC.SuppressFinalize(this._XPathStream);
            }
            GC.SuppressFinalize(this);
        }
    }
}