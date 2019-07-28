namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestQueryString : KeyValueCollection<string, string>, Basics.Context.Request.IHttpRequestQueryString
    {
        private readonly Basics.Context.IUrl _Url;

        public HttpRequestQueryString(Basics.Context.IUrl url)
        {
            if (url == null)
                url = new Url("/");

            this._Url = url;
            this.Parse();
        }

        private void Parse()
        {
            if (string.IsNullOrEmpty(this._Url.QueryString))
                return;
        
            string[] keyValues = this._Url.QueryString.Split('&');

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
                    value = $"{base[key]},{value}";

                base.AddOrUpdate(key, value);
            }
        }
    }
}
