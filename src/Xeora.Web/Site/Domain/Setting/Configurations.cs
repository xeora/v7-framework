using System;
using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Configurations : Basics.Domain.IConfigurations
    {
        private readonly XPathNavigator _XPathNavigator;

        public Configurations(ref XPathNavigator configurationNavigator) =>
            this._XPathNavigator = configurationNavigator.Clone();

        public string AuthenticationTemplate
        {
            get
            {
                string authenticationTemplate = 
                    this.ReadConfiguration("authenticationTemplate");

                if (authenticationTemplate == null)
                    authenticationTemplate = this.DefaultTemplate;

                return authenticationTemplate;
            }
        }

        public string DefaultTemplate => this.ReadConfiguration("defaultTemplate");
        public string DefaultLanguage => this.ReadConfiguration("defaultLanguage");

        public Basics.Enum.PageCachingTypes DefaultCaching
        {
            get
            {
                string configString = this.ReadConfiguration("defaultCaching");

                if (!Enum.TryParse<Basics.Enum.PageCachingTypes>(configString, out Basics.Enum.PageCachingTypes rPageCaching))
                    rPageCaching = Basics.Enum.PageCachingTypes.AllContent;

                return rPageCaching;
            }
        }

        public string LanguageExecutable => this.ReadConfiguration("languageExecutable");
        public string SecurityExecutable => this.ReadConfiguration("securityExecutable");

        private string ReadConfiguration(string key)
        {
            try
            {
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select(string.Format("//Configuration/Item[@key='{0}']", key));

                return xPathIter.MoveNext() ? xPathIter.Current.GetAttribute("value", xPathIter.Current.BaseURI) : null;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}