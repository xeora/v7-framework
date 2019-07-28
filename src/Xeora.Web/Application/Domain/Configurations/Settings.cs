using System.IO;
using System.Xml.XPath;

namespace Xeora.Web.Application.Domain.Configurations
{
    public class Settings : Basics.Domain.ISettings
    {
        private readonly StringReader _XPathStream;

        public Settings(string configurationContent)
        {
            if (string.IsNullOrWhiteSpace(configurationContent))
                throw new System.Exception(Global.SystemMessages.CONFIGURATIONCONTENT + "!");

            XPathNavigator xPathNavigator;
            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(configurationContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                xPathNavigator = xPathDoc.CreateNavigator();
                // !--
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }

            this.Configurations = new ConfigurationManager(ref xPathNavigator);
            this.Services = new Services(ref xPathNavigator);
            this.Mappings = new Mappings(ref xPathNavigator);
        }

        public Basics.Domain.IConfigurations Configurations { get; }
        public Basics.Domain.IServices Services { get; }
        public Basics.Domain.IURL Mappings { get; }

        public void Dispose() => _XPathStream?.Close();
    }
}