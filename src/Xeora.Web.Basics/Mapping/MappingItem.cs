namespace Xeora.Web.Basics.Mapping
{
    public class MappingItem
    {
        public MappingItem()
        {
            this.Overridable = false;
            this.Priority = 0;
            this.RequestMap = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Xeora.Web.Basics.URLMapping.URLMappingItem"/>
        /// is overridable. If it is, sub domain URLMappingItem can override.
        /// </summary>
        /// <value><c>true</c> if overridable; otherwise, <c>false</c></value>
        public bool Overridable { get; set; }

        /// <summary>
        /// Gets or sets the priority of URLMappingItem on Url resolution
        /// </summary>
        /// <value>The priority</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the url map to resolve the request. It supports RegEx.
        /// </summary>
        /// <value>The url resolver RegEx</value>
        public string RequestMap { get; set; }

        /// <summary>
        /// Gets or sets the resolve entry of Xeora service
        /// </summary>
        /// <value>The resolve entry</value>
        public ResolveEntry ResolveEntry { get; set; }
    }
}
