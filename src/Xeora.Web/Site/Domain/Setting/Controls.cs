using System;
using System.IO;
using System.Xml.XPath;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Site.Setting
{
    public class Controls : IControls
    {
        private StringReader _XPathStream = null;
        private XPathNavigator _XPathNavigator;

        internal Controls(string xmlContent)
        {
            if (xmlContent == null || xmlContent.Trim().Length == 0)
                throw new System.Exception(Global.SystemMessages.CONTROLSCONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(xmlContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public XPathNavigator Select(string controlID) =>
            this._XPathNavigator.SelectSingleNode(string.Format("/Controls/Control[@id='{0}']", controlID));

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