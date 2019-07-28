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
        AllContentCookieless,

        /// <summary>
        /// Caches only texts
        /// </summary>
        TextsOnly,

        /// <summary>
        /// Caches only texts without using cookies
        /// </summary>
        TextsOnlyCookieless,

        /// <summary>
        /// Caches nothing
        /// </summary>
        NoCache,

        /// <summary>
        /// Caches nothing and do not use cookies
        /// </summary>
        NoCacheCookieless
    }
}
