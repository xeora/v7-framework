namespace Xeora.Web.Basics.Enum
{
    public enum PageCachingTypes
    {
        /// <summary>
        /// Caches all content
        /// </summary>
        AllContent,

        /// <summary>
        /// Caches all content without using cookies
        /// </summary>
        AllContentCookiless,

        /// <summary>
        /// Caches only texts
        /// </summary>
        TextsOnly,

        /// <summary>
        /// Caches only texts without using cookies
        /// </summary>
        TextsOnlyCookiless,

        /// <summary>
        /// Caches nothing
        /// </summary>
        NoCache,

        /// <summary>
        /// Caches nothing and do not use cookies
        /// </summary>
        NoCacheCookiless
    }

    public enum RequestTagFilteringTypes
    {
        /// <summary>
        /// Filters nothing
        /// </summary>
        None,

        /// <summary>
        /// Filters only form fields
        /// </summary>
        OnlyForm,

        /// <summary>
        /// Filters only query string fields
        /// </summary>
        OnlyQuery,

        /// <summary>
        /// Filters both form and query string fields
        /// </summary>
        Both
    }
}
