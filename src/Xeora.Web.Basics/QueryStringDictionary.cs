using System.Text;
using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public class QueryStringDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Resolves the query string items of current request
        /// </summary>
        /// <returns>The query string items</returns>
        public static QueryStringDictionary Resolve() =>
            QueryStringDictionary.Resolve(Helpers.Context.Request.Header.URL.QueryString);

        /// <summary>
        /// Resolves the query string items
        /// </summary>
        /// <returns>The query string items</returns>
        /// <param name="queryString">Query string</param>
        public static QueryStringDictionary Resolve(string queryString)
        {
            QueryStringDictionary queryStringDictionary =
                new QueryStringDictionary();

            if (string.IsNullOrEmpty(queryString))
                return queryStringDictionary;

            string key = null, value = null;

            foreach (string queryStringItem in queryString.Split('&'))
            {
                string[] splittedQueryStringItem = queryStringItem.Split('=');

                key = splittedQueryStringItem[0];
                value = string.Join("=", splittedQueryStringItem, 1, splittedQueryStringItem.Length - 1);

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    queryStringDictionary[key] = value;
            }

            return queryStringDictionary;
        }

        /// <summary>
        /// Make the query string dictionary using keyvalue pairs
        /// </summary>
        /// <returns>The query string dictionary</returns>
        /// <param name="queryStrings">Query strings</param>
        public static QueryStringDictionary Make(params KeyValuePair<string, string>[] queryStrings)
        {
            QueryStringDictionary queryStringDictionary =
                new QueryStringDictionary();

            if (queryStrings == null)
                return queryStringDictionary;

            foreach (KeyValuePair<string, string> item in queryStrings)
            {
                if (!item.Equals(null) &&
                    !string.IsNullOrEmpty(item.Key) &&
                    !string.IsNullOrEmpty(item.Value))
                    queryStringDictionary[item.Key] = item.Value;
            }

            return queryStringDictionary;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.QueryStringDictionary"/> key values
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.QueryStringDictionary"/> key values</returns>
        public override string ToString()
        {
            StringBuilder rSB =
                new StringBuilder();

            IEnumerator<KeyValuePair<string, string>> enumerator =
                this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (rSB.Length > 0)
                    rSB.Append("&");

                rSB.AppendFormat("{0}={1}", enumerator.Current.Key, enumerator.Current.Value);
            }

            return rSB.ToString();
        }
    }
}
