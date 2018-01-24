using System;
using System.IO;
using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Settings : Basics.Domain.ISettings
    {
        private StringReader _XPathStream = null;
        private XPathNavigator _XPathNavigator;

        public Settings(string configurationContent)
        {
            if (string.IsNullOrWhiteSpace(configurationContent))
                throw new System.Exception(Global.SystemMessages.CONFIGURATIONCONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(configurationContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }

            this.Configurations = new Configurations(ref this._XPathNavigator);
            this.Services = new Services(ref this._XPathNavigator);
            this.Mappings = new Mappings(ref this._XPathNavigator);
        }

        public Basics.Domain.IConfigurations Configurations { get; private set; }
        public Basics.Domain.IServices Services { get; private set; }
        public Basics.Domain.IURL Mappings { get; private set; }

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