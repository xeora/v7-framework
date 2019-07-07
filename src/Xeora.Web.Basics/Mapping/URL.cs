using System.Collections.Concurrent;

namespace Xeora.Web.Basics.Mapping
{
    public class URL
    {
        private static ConcurrentDictionary<string[], URL> _Mappings =
            new ConcurrentDictionary<string[], URL>();

        private URL(bool active, string resolverExecutable, MappingItem[] items)
        {
            this.Active = active;
            this.ResolverExecutable = resolverExecutable;
            this.Items = new MappingItemCollection(items);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Xeora.Web.Basics.Mapping.URL"/> is active
        /// </summary>
        /// <value><c>true</c> if is active; otherwise, <c>false</c></value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the resolver executable
        /// </summary>
        /// <value>The resolver executable</value>
        public string ResolverExecutable { get; set; }

        /// <summary>
        /// Gets the query items
        /// </summary>
        /// <value>Query items</value>
        public MappingItemCollection Items { get; private set; }

        /// <summary>
        /// Gets the current URL Mapping definition instance
        /// </summary>
        /// <value>The current URL Mapping definition instance</value>
        public static URL Current
        {
            get
            {
                URL urlInstance = null;
                if (!URL._Mappings.TryGetValue(Helpers.CurrentDomainInstance.IdAccessTree, out urlInstance))
                {
                    urlInstance =
                        new URL(
                            Helpers.CurrentDomainInstance.Settings.Mappings.Active,
                            Helpers.CurrentDomainInstance.Settings.Mappings.ResolverExecutable,
                            Helpers.CurrentDomainInstance.Settings.Mappings.Items.ToArray()
                        );

                    if (!URL._Mappings.TryAdd(Helpers.CurrentDomainInstance.IdAccessTree, urlInstance))
                        return URL.Current;
                }

                return urlInstance;
            }
        }

        /// <summary>
        /// Resolves the URL according to the URL Mapping definitions
        /// </summary>
        /// <returns>The URL resolution</returns>
        /// <param name="requestFilePath">Request file path</param>
        public ResolutionResult ResolveURL(string requestFilePath)
        {
            ResolutionResult resolutionResult = null;

            if (this.Active)
            {
                resolutionResult =
                    Helpers.HandlerInstance.DomainControl.ResolveURL(requestFilePath);

                if (resolutionResult == null || !resolutionResult.Resolved)
                {
                    foreach (MappingItem item in this.Items)
                    {
                        System.Text.RegularExpressions.Match rqMatch =
                            System.Text.RegularExpressions.Regex.Match(requestFilePath, item.RequestMap, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        if (rqMatch.Success)
                        {
                            resolutionResult = new ResolutionResult(true, item.ResolveEntry.ServiceDefinition);

                            string resolveItemValue;
                            foreach (ResolveItem resolveItem in item.ResolveEntry.ResolveItems)
                            {
                                resolveItemValue = string.Empty;

                                if (!string.IsNullOrEmpty(resolveItem.Id))
                                    resolveItemValue = rqMatch.Groups[resolveItem.Id].Value;

                                resolutionResult.QueryString[resolveItem.QueryStringKey] =
                                    string.IsNullOrEmpty(resolveItemValue) ? resolveItem.DefaultValue : resolveItemValue;
                            }

                            break;
                        }
                    }
                }

                if (resolutionResult == null)
                    resolutionResult = new ResolutionResult(false, null);
            }

            return resolutionResult;
        }
    }
}
