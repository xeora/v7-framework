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
            QueryStringDictionary.Resolve(Helpers.Context.Request.Header.Url.QueryString);

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

            foreach (string queryStringItem in queryString.Split('&'))
            {
                string[] queryStringItems = queryStringItem.Split('=');

                string key = queryStringItems[0];
                string value = 
                    string.Join("=", queryStringItems, 1, queryStringItems.Length - 1);

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    queryStringDictionary[key] = value;
            }

            return queryStringDictionary;
        }

        /// <summary>
        /// Make the query string dictionary using key-value pairs
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
            StringBuilder sB =
                new StringBuilder();

            IEnumerator<KeyValuePair<string, string>> enumerator =
                this.GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    if (sB.Length > 0)
                        sB.Append("&");

                    sB.AppendFormat("{0}={1}", enumerator.Current.Key, enumerator.Current.Value);
                }
                
                return sB.ToString();
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
