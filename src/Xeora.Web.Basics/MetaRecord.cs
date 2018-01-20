namespace Xeora.Web.Basics
{
    public class MetaRecord
    {
        public enum Tags
        {
            author,
            cachecontrol,
            contentlanguage,
            contenttype,
            copyright,
            description,
            expires,
            keywords,
            pragma,
            refresh,
            robots,
            googlebot
        }

        public enum TagSpaces
        {
            name,
            httpequiv,
            property
        }

        /// <summary>
        /// Gets the html name of meta tag
        /// </summary>
        /// <returns>The html name of meta tag</returns>
        /// <param name="tag">MetaTag</param>
        public static string GetTagHtmlName(Tags tag)
        {
            switch (tag)
            {
                case Tags.author:
                    return "Author";
                case Tags.cachecontrol:
                    return "Cache-Control";
                case Tags.contentlanguage:
                    return "Content-Language";
                case Tags.contenttype:
                    return "Content-Type";
                case Tags.copyright:
                    return "Copyright";
                case Tags.description:
                    return "Description";
                case Tags.expires:
                    return "Expires";
                case Tags.googlebot:
                    return "Googlebot";
                case Tags.keywords:
                    return "Keywords";
                case Tags.pragma:
                    return "Pragma";
                case Tags.refresh:
                    return "Refresh";
                case Tags.robots:
                    return "Robots";
            }

            return string.Empty;
        }

        /// <summary>
        /// Queries the tag space to find which meta tag space it belongs to
        /// </summary>
        /// <returns>The tag space</returns>
        /// <param name="tag">MetaTag</param>
        public static TagSpaces QueryTagSpace(Tags tag)
        {
            switch (tag)
            {
                case Tags.author:
                case Tags.copyright:
                case Tags.description:
                case Tags.keywords:
                case Tags.robots:
                case Tags.googlebot:
                    return TagSpaces.name;
                default:
                    return TagSpaces.httpequiv;
            }
        }

        /// <summary>
        /// Queries the tag space to find which meta tag space it belongs to according to meta name input
        /// </summary>
        /// <returns>The tag space</returns>
        /// <param name="name">MetaName</param>
        public static TagSpaces QueryTagSpace(ref string name)
        {
            if (string.IsNullOrEmpty(name))
                name = string.Empty;

            if (name.IndexOf("name::") == 0)
            {
                name = name.Substring(6);
                return TagSpaces.name;
            }
            else if (name.IndexOf("httpequiv::") == 0)
            {
                name = name.Substring(11);
                return TagSpaces.httpequiv;
            }
            else if (name.IndexOf("property::") == 0)
            {
                name = name.Substring(10);
                return TagSpaces.property;
            }

            return default(TagSpaces);
        }

        /// <summary>
        /// Gets the meta records instance
        /// </summary>
        /// <value>The instance of meta records</value>
        public static IMetaRecordCollection Records => Helpers.HandlerInstance.DomainControl.MetaRecord;
    }
}
