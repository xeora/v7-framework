using System;
using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Configurations : Basics.IConfigurations
    {
        private XPathNavigator _XPathNavigator;

        public Configurations(ref XPathNavigator configurationNavigator) =>
            this._XPathNavigator = configurationNavigator.Clone();

        public string AuthenticationPage
        {
            get
            {
                string authenticationPage = 
                    this.ReadConfiguration("authenticationpage");

                if (authenticationPage == null)
                    authenticationPage = this.DefaultPage;

                return authenticationPage;
            }
        }

        public string DefaultPage => this.ReadConfiguration("defaultpage");
        public string DefaultLanguage => this.ReadConfiguration("defaultlanguage");

        public Basics.Enum.PageCachingTypes DefaultCaching
        {
            get
            {
                Basics.Enum.PageCachingTypes rPageCaching;
                string configString = this.ReadConfiguration("defaultcaching");

                if (!Enum.TryParse<Basics.Enum.PageCachingTypes>(configString, out rPageCaching))
                    rPageCaching = Basics.Enum.PageCachingTypes.AllContent;

                return rPageCaching;
            }
        }

        public string DefaultSecurityBind => this.ReadConfiguration("defaultsecuritybind");

        private string ReadConfiguration(string key)
        {
            try
            {
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select(string.Format("//Configuration/Item[@key='{0}']", key));

                if (xPathIter.MoveNext())
                    return xPathIter.Current.GetAttribute("value", xPathIter.Current.BaseURI);
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }

            return null;
        }
    }
}