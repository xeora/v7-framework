using System;

namespace Xeora.Web.Basics
{
    public static class MetaRecord
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
            return tag switch
            {
                Tags.author => "Author",
                Tags.cachecontrol => "Cache-Control",
                Tags.contentlanguage => "Content-Language",
                Tags.contenttype => "Content-Type",
                Tags.copyright => "Copyright",
                Tags.description => "Description",
                Tags.expires => "Expires",
                Tags.googlebot => "Googlebot",
                Tags.keywords => "Keywords",
                Tags.pragma => "Pragma",
                Tags.refresh => "Refresh",
                Tags.robots => "Robots",
                _ => string.Empty
            };
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

            if (name.IndexOf("name::", StringComparison.Ordinal) == 0)
            {
                name = name.Substring(6);
                return TagSpaces.name;
            }
            
            if (name.IndexOf("httpequiv::", StringComparison.Ordinal) == 0)
            {
                name = name.Substring(11);
                return TagSpaces.httpequiv;
            }
            
            if (name.IndexOf("property::", StringComparison.Ordinal) == 0)
            {
                name = name.Substring(10);
                return TagSpaces.property;
            }

            return default;
        }

        /// <summary>
        /// Gets the meta records instance
        /// </summary>
        /// <value>The instance of meta records</value>
        public static IMetaRecordCollection Records => Helpers.HandlerInstance.DomainControl.MetaRecord;
    }
}
