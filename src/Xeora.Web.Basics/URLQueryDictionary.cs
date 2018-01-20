using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    [Serializable()]
    public class URLQueryDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Resolves the query string items of current request
        /// </summary>
        /// <returns>The query string items</returns>
        public static URLQueryDictionary ResolveQueryItems() =>
            URLQueryDictionary.ResolveQueryItems(Helpers.Context.Request.Header.URL.QueryString);

        /// <summary>
        /// Resolves the query string items
        /// </summary>
        /// <returns>The query string items</returns>
        /// <param name="queryString">Query string</param>
        public static URLQueryDictionary ResolveQueryItems(string queryString)
        {
            URLQueryDictionary urlQueryDictionary = new URLQueryDictionary();

            if (!string.IsNullOrEmpty(queryString))
            {
                string key = null, value = null;

                foreach (string queryStringItem in queryString.Split('&'))
                {
                    string[] splittedQueryStringItem = queryStringItem.Split('=');

                    key = splittedQueryStringItem[0];
                    value = string.Join("=", splittedQueryStringItem, 1, splittedQueryStringItem.Length - 1);

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        urlQueryDictionary[key] = value;
                }
            }

            return urlQueryDictionary;
        }

        /// <summary>
        /// Make the query string dictionary using keyvalue pairs
        /// </summary>
        /// <returns>The query string dictionary</returns>
        /// <param name="queryStrings">Query strings</param>
        public static URLQueryDictionary Make(params KeyValuePair<string, string>[] queryStrings)
        {
            URLQueryDictionary urlQueryDictionary = new URLQueryDictionary();

            if (queryStrings != null)
            {
                foreach (KeyValuePair<string, string> item in queryStrings)
                {
                    if (!item.Equals(null) && 
                        !string.IsNullOrEmpty(item.Key) && 
                        !string.IsNullOrEmpty(item.Value))
                        urlQueryDictionary[item.Key] = item.Value;
                }
            }

            return urlQueryDictionary;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.URLQueryDictionary"/> key values
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.URLQueryDictionary"/> key values</returns>
        public override string ToString()
        {
            System.Text.StringBuilder rSB = new System.Text.StringBuilder();

            IEnumerator<KeyValuePair<string, string>> enumerator = this.GetEnumerator();

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
