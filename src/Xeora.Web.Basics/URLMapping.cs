using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Xeora.Web.Basics
{
    public class URLMapping
    {
        private static ConcurrentDictionary<string[], URLMapping> _URLMappings =
            new ConcurrentDictionary<string[], URLMapping>();

        private URLMapping(bool isActive, string resolverExecutable, URLMappingItem[] mapItems)
        {
            this.IsActive = isActive;
            this.ResolverBindInfo = Execution.BindInfo.Make(resolverExecutable);
            this.Items = new URLMappingItem.URLMappingItemCollection();
            this.Items.AddRange(mapItems);
        }

        public bool IsActive { get; set; }
        public Execution.BindInfo ResolverBindInfo { get; set; }
        public URLMappingItem.URLMappingItemCollection Items { get; private set; }

        public static URLMapping Current
        {
            get
            {
                URLMapping rURLMappingInstance = null;
                if (!URLMapping._URLMappings.TryGetValue(Helpers.CurrentDomainInstance.IDAccessTree, out rURLMappingInstance))
                {
                    URLMapping urlMappingInstance =
                        new URLMapping(
                            Helpers.CurrentDomainInstance.Settings.URLMappings.IsActive,
                            Helpers.CurrentDomainInstance.Settings.URLMappings.ResolverExecutable,
                            Helpers.CurrentDomainInstance.Settings.URLMappings.Items.ToArray()
                        );

                    if (!URLMapping._URLMappings.TryAdd(Helpers.CurrentDomainInstance.IDAccessTree, urlMappingInstance))
                        return URLMapping.Current;
                }

                return rURLMappingInstance;
            }
        }

        public ResolvedMapped ResolveMappedURL(string requestFilePath)
        {
            ResolvedMapped rResolvedMapped = null;

            if (this.IsActive)
            {
                rResolvedMapped =
                    Helpers.HandlerInstance.DomainControl.QueryURLResolver(requestFilePath);

                if (rResolvedMapped == null || !rResolvedMapped.IsResolved)
                {
                    foreach (URLMappingItem urlMapItem in this.Items)
                    {
                        System.Text.RegularExpressions.Match rqMatch =
                            System.Text.RegularExpressions.Regex.Match(requestFilePath, urlMapItem.RequestMap, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        if (rqMatch.Success)
                        {
                            rResolvedMapped = new ResolvedMapped(true, urlMapItem.ResolveInfo.ServicePathInfo);

                            string medItemValue;
                            foreach (ResolveInfos.MappedItem medItem in urlMapItem.ResolveInfo.MappedItems)
                            {
                                medItemValue = string.Empty;

                                if (!string.IsNullOrEmpty(medItem.ID))
                                    medItemValue = rqMatch.Groups[medItem.ID].Value;

                                rResolvedMapped.URLQueryDictionary[medItem.QueryStringKey] =
                                    string.IsNullOrEmpty(medItemValue) ? medItem.DefaultValue : medItemValue;
                            }

                            break;
                        }
                    }
                }

                if (rResolvedMapped == null)
                    rResolvedMapped = new ResolvedMapped(false, null);
            }

            return rResolvedMapped;
        }

        public class URLMappingItem
        {
            public URLMappingItem()
            {
                this.Overridable = false;
                this.Priority = 0;
                this.RequestMap = string.Empty;
            }

            public bool Overridable { get; set; }
            public int Priority { get; set; }
            public string RequestMap { get; set; }
            public ResolveInfos ResolveInfo { get; set; }

            public class URLMappingItemCollection : List<URLMappingItem>
            {
                public new void Add(URLMappingItem item)
                {
                    base.Add(item);
                    this.Sort();
                }

                public new void AddRange(IEnumerable<URLMappingItem> collection)
                {
                    base.AddRange(collection);
                    this.Sort();
                }

                public new void Sort() =>
                    base.Sort(new PriorityComparer());

                private class PriorityComparer : IComparer<URLMappingItem>
                {
                    public int Compare(URLMappingItem x, URLMappingItem y)
                    {
                        if (x.Priority > y.Priority)
                            return -1;
                        if (x.Priority < y.Priority)
                            return 1;

                        return 0;
                    }
                }
            }
        }

        public class ResolvedMapped
        {
            public ResolvedMapped(bool isResolved, ServicePathInfo servicePathInfo)
            {
                this.IsResolved = isResolved;

                this.ServicePathInfo = servicePathInfo;
                this.URLQueryDictionary = new URLQueryDictionary();
            }

            public bool IsResolved { get; private set; }
            public ServicePathInfo ServicePathInfo { get; private set; }
            public URLQueryDictionary URLQueryDictionary { get; private set; }
        }

        public class ResolveInfos
        {
            public ResolveInfos(ServicePathInfo servicePathInfo)
            {
                this.ServicePathInfo = servicePathInfo;
                this.MapFormat = string.Empty;
                this.MappedItems = new MappedItem.MappedItemCollection();
            }

            public ServicePathInfo ServicePathInfo { get; private set; }
            public string MapFormat { get; set; }
            public MappedItem.MappedItemCollection MappedItems { get; private set; }

            public class MappedItem
            {
                public MappedItem(string ID)
                {
                    this.ID = ID;
                    this.DefaultValue = string.Empty;
                    this.QueryStringKey = ID;
                }

                public string ID { get; private set; }
                public string DefaultValue { get; set; }
                public string QueryStringKey { get; set; }

                public class MappedItemCollection : List<MappedItem>
                {
                    public MappedItem this[string ID]
                    {
                        get
                        {
                            foreach (MappedItem medItem in this)
                            {
                                if (string.Compare(medItem.ID, ID, true) == 0)
                                    return medItem;
                            }

                            return new MappedItem(ID);
                        }
                    }
                }
            }
        }
    }
}
