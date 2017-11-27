using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    [Serializable()]
    public class URLQueryDictionary : Dictionary<string, string>
    {
        public static URLQueryDictionary ResolveQueryItems() =>
            URLQueryDictionary.ResolveQueryItems(Helpers.Context.Request.Header.URL.QueryString);

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
