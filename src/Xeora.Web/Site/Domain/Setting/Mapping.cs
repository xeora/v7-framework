using System.Xml.XPath;

namespace Xeora.Web.Site.Setting
{
    public class Mappings : Basics.Domain.IURL
    {
        private readonly XPathNavigator _XPathNavigator;

        public Mappings(ref XPathNavigator configurationNavigator)
        {
            this._XPathNavigator = 
                configurationNavigator.Clone();
            this.PrepareOptions();
        }

        public bool Active { get; private set; }
        public string ResolverExecutable { get; private set; }
        public Basics.Mapping.MappingItemCollection Items { get; private set; }

        private void PrepareOptions()
        {
            this.Items = 
                new Basics.Mapping.MappingItemCollection(null);

            try
            {
                XPathNodeIterator xPathIter = 
                    this._XPathNavigator.Select(string.Format("//Mapping"));

                if (xPathIter.MoveNext())
                {
                    if (!bool.TryParse(xPathIter.Current.GetAttribute("active", xPathIter.Current.BaseURI), out bool isActive))
                        this.Active = false;
                    this.Active = isActive;

                    this.ResolverExecutable = 
                        xPathIter.Current.GetAttribute("resolverExecutable", xPathIter.Current.BaseURI);
                }

                // If mapping is active then read mapping items
                if (this.Active)
                {
                    // Read Mapping Options
                    xPathIter = this._XPathNavigator.Select(string.Format("//Mapping/Map"));

                    int priority = 0;
                    string request = string.Empty;

                    string reverseID = string.Empty, reverseMapped = string.Empty;
                    Basics.Mapping.ResolveItemCollection reverseMappedItems = null;
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
                                        reverseMappedItems = new Basics.Mapping.ResolveItemCollection();

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

                                                        Basics.Mapping.ResolveItem resolveItem =
                                                            new Basics.Mapping.ResolveItem(mappedItemID)
                                                            {
                                                                QueryStringKey = mappedItemQueryStringKey,
                                                                DefaultValue = mappedItemDefaultValue
                                                            };

                                                        reverseMappedItems.Add(resolveItem);

                                                        break;
                                                }
                                            } while (xPathIterSub.Current.MoveToNext());
                                        }

                                        break;
                                }
                            } while (xPathIterSub.Current.MoveToNext());
                        }

                        Basics.Mapping.MappingItem tMappingItem =
                            new Basics.Mapping.MappingItem
                            {
                                Overridable = overridable,
                                Priority = priority,
                                RequestMap = request
                            };

                        Basics.Mapping.ResolveEntry resolveEntry =
                            new Basics.Mapping.ResolveEntry(
                                Basics.ServiceDefinition.Parse(reverseID, true))
                            {
                                MapFormat = reverseMapped
                            };
                        resolveEntry.ResolveItems.AddRange(reverseMappedItems);

                        tMappingItem.ResolveEntry = resolveEntry;

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