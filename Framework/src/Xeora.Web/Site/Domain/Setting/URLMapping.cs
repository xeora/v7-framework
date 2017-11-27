using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class URLMapping : Basics.IURLMappings
    {
        private XPathNavigator _XPathNavigator;

        public URLMapping(ref XPathNavigator configurationNavigator)
        {
            this._XPathNavigator = 
                configurationNavigator.Clone();
            this.PrepareOptions();
        }

        public bool IsActive { get; private set; }
        public string ResolverExecutable { get; private set; }
        public Basics.URLMapping.URLMappingItem.URLMappingItemCollection Items { get; private set; }

        private void PrepareOptions()
        {
            this.Items = 
                new Basics.URLMapping.URLMappingItem.URLMappingItemCollection();

            try
            {
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select(string.Format("//URLMapping"));

                if (xPathIter.MoveNext())
                {
                    bool isActive = true;
                    if (!bool.TryParse(xPathIter.Current.GetAttribute("active", xPathIter.Current.BaseURI), out isActive))
                        this.IsActive = false;
                    this.IsActive = isActive;

                    this.ResolverExecutable = 
                        xPathIter.Current.GetAttribute("resolverExecutable", xPathIter.Current.BaseURI);
                }

                // If mapping is active then read mapping items
                if (this.IsActive)
                {
                    // Read URLMapping Options
                    xPathIter = this._XPathNavigator.Select(string.Format("//URLMapping/Map"));

                    int priority = 0;
                    string request = string.Empty;

                    string reverseID = string.Empty, reverseMapped = string.Empty;
                    Basics.URLMapping.ResolveInfos.MappedItem.MappedItemCollection reverseMappedItems = null;
                    bool overridable = false;

                    while (xPathIter.MoveNext())
                    {
                        int.TryParse(
                            xPathIter.Current.GetAttribute("priority", xPathIter.Current.BaseURI), out priority);

                        XPathNodeIterator xPathIterSub = xPathIter.Clone();

                        if (xPathIterSub.Current.MoveToFirstChild())
                        {
                            do
                            {
                                switch (xPathIterSub.Current.Name)
                                {
                                    case "Request":
                                        xPathIterSub.Current.MoveToFirstChild();

                                        request = xPathIterSub.Current.Value;
                                        request = request.Replace("~/", Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);

                                        xPathIterSub.Current.MoveToParent();

                                        break;
                                    case "Reverse":
                                        reverseID = xPathIterSub.Current.GetAttribute("id", xPathIterSub.Current.BaseURI);

                                        XPathNodeIterator xPathIter_servicetest = 
                                            this._XPathNavigator.Select(string.Format("//Services/Item[@id='{0}']", reverseID));

                                        if (xPathIter_servicetest.MoveNext())
                                            bool.TryParse(
                                                xPathIter.Current.GetAttribute("overridable", xPathIter.Current.BaseURI), out overridable);

                                        // TODO: mapped is not in use. The logic was creating the formatted URL from resolved request.
                                        reverseMapped = xPathIterSub.Current.GetAttribute("mapped", xPathIterSub.Current.BaseURI);
                                        reverseMappedItems = new Basics.URLMapping.ResolveInfos.MappedItem.MappedItemCollection();

                                        if (xPathIterSub.Current.MoveToFirstChild())
                                        {
                                            do
                                            {
                                                switch (xPathIterSub.Current.Name)
                                                {
                                                    case "MappedItem":
                                                        string mappedItemID =
                                                            xPathIterSub.Current.GetAttribute("id", xPathIterSub.Current.BaseURI);
                                                        string mappedItemQueryStringKey =
                                                            xPathIterSub.Current.GetAttribute("key", xPathIterSub.Current.BaseURI);
                                                        string mappedItemDefaultValue =
                                                            xPathIterSub.Current.GetAttribute("defaultValue", xPathIterSub.Current.BaseURI);

                                                        Basics.URLMapping.ResolveInfos.MappedItem mappedItem = 
                                                            new Basics.URLMapping.ResolveInfos.MappedItem(mappedItemID);

                                                        mappedItem.QueryStringKey = mappedItemQueryStringKey;
                                                        mappedItem.DefaultValue = mappedItemDefaultValue;

                                                        reverseMappedItems.Add(mappedItem);

                                                        break;
                                                }
                                            } while (xPathIterSub.Current.MoveToNext());
                                        }

                                        break;
                                }
                            } while (xPathIterSub.Current.MoveToNext());
                        }

                        Basics.URLMapping.URLMappingItem tMappingItem = 
                            new Basics.URLMapping.URLMappingItem();

                        tMappingItem.Overridable = overridable;
                        tMappingItem.Priority = priority;
                        tMappingItem.RequestMap = request;

                        Basics.URLMapping.ResolveInfos resInfo = 
                            new Basics.URLMapping.ResolveInfos(
                                Basics.ServicePathInfo.Parse(reverseID, true));

                        resInfo.MapFormat = reverseMapped;
                        resInfo.MappedItems.AddRange(reverseMappedItems);

                        tMappingItem.ResolveInfo = resInfo;

                        this.Items.Add(tMappingItem);
                    }
                }
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }
        }
    }
}