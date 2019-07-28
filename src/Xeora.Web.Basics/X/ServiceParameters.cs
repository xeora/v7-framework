using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Xeora.Web.Basics.X
{
    public class ServiceParameterCollection : Dictionary<string, string>
    {
        public ServiceParameterCollection() =>
            this.PublicKey = string.Empty;

        public string PublicKey { get; set; }

        public void ParseXML(string serviceParametersXML)
        {
            this.Clear();

            if (string.IsNullOrEmpty(serviceParametersXML))
                return;

            StringReader xPathTextReader = null;
            try
            {
                xPathTextReader = new StringReader(serviceParametersXML);
                XPathDocument xPathDoc = new XPathDocument(xPathTextReader);

                XPathNavigator xPathNavigator = xPathDoc.CreateNavigator();
                XPathNodeIterator xPathIter =
                    xPathNavigator.Select("/ServiceParameters/Item");

                string key = null, value = null;
                while (xPathIter.MoveNext())
                {
                    key = xPathIter.Current.GetAttribute("key", xPathIter.Current.NamespaceURI);
                    value = xPathIter.Current.Value;

                    if (string.Compare(key, "PublicKey") == 0)
                    {
                        this.PublicKey = value;

                        continue;
                    }

                    base[key] = value;
                }
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
            finally
            {
                xPathTextReader?.Close();
            }
        }

        public string ToXML()
        {
            StringWriter xmlStream = null;
            System.Xml.XmlTextWriter xmlWriter = null;
            try
            {
                xmlStream = new StringWriter();
                xmlWriter = new System.Xml.XmlTextWriter(xmlStream);

                // Start Document Element
                xmlWriter.WriteStartElement("ServiceParameters");

                if (this.PublicKey != null)
                {
                    xmlWriter.WriteStartElement("Item");
                    xmlWriter.WriteAttributeString("key", "PublicKey");
                    xmlWriter.WriteString(this.PublicKey);
                    xmlWriter.WriteEndElement();
                }

                Enumerator enumerator = this.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    xmlWriter.WriteStartElement("Item");
                    xmlWriter.WriteAttributeString("key", enumerator.Current.Key);
                    xmlWriter.WriteCData(enumerator.Current.Value);
                    xmlWriter.WriteEndElement();
                }

                // End Document Element
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();

                return xmlStream.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (xmlWriter != null)
                    xmlWriter.Close();

                if (xmlStream != null)
                    xmlStream.Close();
            }
        }
    }
}
