namespace Xeora.Web.Service.Context
{
    public class HttpRequestQueryString : KeyValueCollection<string, string>, Basics.Context.IHttpRequestQueryString
    {
        private readonly Basics.Context.IURL _URL;

        public HttpRequestQueryString(Basics.Context.IURL url)
        {
            if (url == null)
                url = new URL("/");

            this._URL = url;
            this.Parse();
        }

        private void Parse()
        {
            if (string.IsNullOrEmpty(this._URL.QueryString))
                return;
        
            string[] keyValues = this._URL.QueryString.Split('&');

            foreach (string keyValue in keyValues)
            {
                int equalsIndex = keyValue.IndexOf('=');
                string key, value = string.Empty;

                if (equalsIndex == -1)
                    key = keyValue;
                else
                {
                    key = keyValue.Substring(0, equalsIndex);
                    value = keyValue.Substring(equalsIndex + 1);

                    value = System.Web.HttpUtility.UrlDecode(value);
                }

                if (base.ContainsKey(key))
                    value = string.Format("{0},{1}", base[key], value);

                base.AddOrUpdate(key, value);
            }
        }
    }
}
