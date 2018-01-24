namespace Xeora.Web.Basics.Enum
{
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
