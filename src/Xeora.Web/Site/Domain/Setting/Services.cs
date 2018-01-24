using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Services : Basics.Domain.IServices
    {
        private XPathNavigator _XPathNavigator;

        public Services(ref XPathNavigator configurationNavigator) =>
            this._XPathNavigator = configurationNavigator.Clone();

        private Basics.Domain.IServiceItemCollection _ServiceItems = null;
        public Basics.Domain.IServiceItemCollection ServiceItems
        {
            get
            {
                if (this._ServiceItems == null)
                    this._ServiceItems = this.ReadServiceOptions();

                return this._ServiceItems;
            }
        }

        private Basics.Domain.IServiceItemCollection ReadServiceOptions()
        {
            ServiceItemCollection rCollection = new ServiceItemCollection();

            try
            {
                // Read Authentication Keys
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select("//Services/AuthenticationKeys/Item");

                List<string> authenticationKeys = new List<string>();

                while (xPathIter.MoveNext())
                {
                    authenticationKeys.Add(
                        xPathIter.Current.GetAttribute("id", xPathIter.Current.BaseURI));
                }

                xPathIter = this._XPathNavigator.Select("//Services/Item");

                ServiceItem tServiceItem;
                Basics.Domain.ServiceTypes type = Basics.Domain.ServiceTypes.Template;
                bool overridable = false, authentication = false, standAlone = false;

                while (xPathIter.MoveNext())
                {
                    tServiceItem = 
                        new ServiceItem(
                            xPathIter.Current.GetAttribute("id", xPathIter.Current.BaseURI));

                    Enum.TryParse<Basics.Domain.ServiceTypes>(
                        xPathIter.Current.GetAttribute("type", xPathIter.Current.BaseURI), out type);
                    tServiceItem.ServiceType = type;

                    bool.TryParse(
                        xPathIter.Current.GetAttribute("overridable", xPathIter.Current.BaseURI), out overridable);
                    tServiceItem.Overridable = overridable;

                    bool.TryParse(
                        xPathIter.Current.GetAttribute("authentication", xPathIter.Current.BaseURI), out authentication);
                    tServiceItem.Authentication = authentication;

                    bool.TryParse(
                        xPathIter.Current.GetAttribute("standalone", xPathIter.Current.BaseURI), out standAlone);
                    tServiceItem.StandAlone = standAlone;

                    tServiceItem.ExecuteIn = 
                        xPathIter.Current.GetAttribute("executein", xPathIter.Current.BaseURI);
                    tServiceItem.AuthenticationKeys = authenticationKeys.ToArray();

                    string mimeType = 
                        xPathIter.Current.GetAttribute("mime", xPathIter.Current.BaseURI);
                    switch (tServiceItem.ServiceType)
                    {
                        case Basics.Domain.ServiceTypes.xSocket:
                            tServiceItem.MimeType = mimeType;
                            if (string.IsNullOrEmpty(mimeType))
                                tServiceItem.MimeType = "application/octet-stream";

                            break;
                        case Basics.Domain.ServiceTypes.xService:
                            tServiceItem.MimeType = "text/xml; charset=utf-8";

                            break;
                        default:
                            if (!string.IsNullOrEmpty(mimeType))
                                tServiceItem.MimeType = mimeType;

                            break;
                    }

                    rCollection.Add(tServiceItem);
                }
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }

            return rCollection;
        }
    }
}