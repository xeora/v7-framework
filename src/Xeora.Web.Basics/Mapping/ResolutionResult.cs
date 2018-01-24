namespace Xeora.Web.Basics.Mapping
{
    public class ResolutionResult
    {
        public ResolutionResult(bool resolved, ServiceDefinition serviceDefinition)
        {
            this.Resolved = resolved;

            this.ServiceDefinition = serviceDefinition;
            this.QueryString = new QueryStringDictionary();
        }

        /// <summary>
        /// Gets a value indicating whether this URL Mapping is resolved
        /// </summary>
        /// <value><c>true</c> if is resolved; otherwise, <c>false</c></value>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Gets the Xeora service definition
        /// </summary>
        /// <value>The service definition</value>
        public ServiceDefinition ServiceDefinition { get; private set; }

        /// <summary>
        /// Gets the URL Query string dictionary
        /// </summary>
        /// <value>The URL Query string dictionary</value>
        public QueryStringDictionary QueryString { get; private set; }
    }
}
