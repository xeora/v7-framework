using System.Collections.Concurrent;

namespace Xeora.Web.Basics.Mapping
{
    public class Url
    {
        private static readonly ConcurrentDictionary<string[], Url> Mappings =
            new ConcurrentDictionary<string[], Url>();

        private Url(bool active, string resolverExecutable, MappingItem[] items)
        {
            this.Active = active;
            this.ResolverExecutable = resolverExecutable;
            this.Items = new MappingItemCollection(items);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Xeora.Web.Basics.Mapping.Url"/> is active
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
        public MappingItemCollection Items { get; }

        /// <summary>
        /// Gets the current Url Mapping definition instance
        /// </summary>
        /// <value>The current Url Mapping definition instance</value>
        public static Url Current
        {
            get
            {
                if (Url.Mappings.TryGetValue(Helpers.CurrentDomainInstance.IdAccessTree, out var urlInstance))
                    return urlInstance;
                
                urlInstance =
                    new Url(
                        Helpers.CurrentDomainInstance.Settings.Mappings.Active,
                        Helpers.CurrentDomainInstance.Settings.Mappings.ResolverExecutable,
                        Helpers.CurrentDomainInstance.Settings.Mappings.Items.ToArray()
                    );

                return !Url.Mappings.TryAdd(Helpers.CurrentDomainInstance.IdAccessTree, urlInstance) 
                        ? Url.Current 
                        : urlInstance;
            }
        }

        /// <summary>
        /// Resolves the Url according to the Url Mapping definitions
        /// </summary>
        /// <returns>The Url resolution</returns>
        /// <param name="requestFilePath">Request file path</param>
        public ResolutionResult ResolveUrl(string requestFilePath)
        {
            if (!this.Active) return null;
            
            ResolutionResult resolutionResult =
                Helpers.HandlerInstance.DomainControl.ResolveUrl(requestFilePath);

            if (resolutionResult != null && resolutionResult.Resolved)
                return resolutionResult;
            
            foreach (MappingItem item in this.Items)
            {
                System.Text.RegularExpressions.Match rqMatch =
                    System.Text.RegularExpressions.Regex.Match(requestFilePath, item.RequestMap, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (!rqMatch.Success) continue;
                
                resolutionResult = 
                    new ResolutionResult(true, item.ResolveEntry.ServiceDefinition);
                
                foreach (ResolveItem resolveItem in item.ResolveEntry.ResolveItems)
                {
                    string resolveItemValue = string.Empty;

                    if (!string.IsNullOrEmpty(resolveItem.Id))
                        resolveItemValue = rqMatch.Groups[resolveItem.Id].Value;

                    resolutionResult.QueryString[resolveItem.QueryStringKey] =
                        string.IsNullOrEmpty(resolveItemValue) ? resolveItem.DefaultValue : resolveItemValue;
                }

                break;
            }

            return resolutionResult;
        }
    }
}
