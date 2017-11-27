using System.IO;
using System.Text;

namespace Xeora.Web.Service.Context
{
    public class HttpRequestHeader : KeyValueCollection<string, string>, Basics.Context.IHttpRequestHeader
    {
        private Stream _RequestInput;

        private Basics.Context.HttpMethod _Method;
        private Basics.Context.IURL _URL;
        private string _Protocol;

        private string _Host = string.Empty;
        private string _UserAgent = string.Empty;
        private int _ContentLength = 0;
        private string _ContentType = string.Empty;
        private Encoding _ContentEncoding = null;
        private string _Boundary = string.Empty;

        private Basics.Context.IHttpCookie _Cookie;

        private bool _EOF = false;

        public HttpRequestHeader(ref Stream requestInput)
        {
            this._RequestInput = requestInput;

            this._Cookie = new HttpCookie();

            this.Parse();
        }

        private string ExtractHeader()
        {
            this._RequestInput.Seek(0, SeekOrigin.Begin);

            string RNRN = "\r\n\r\n";
            string NN = "\n\n";
            int NL;

            byte[] buffer = new byte[1024];
            string content = string.Empty;

            do
            {
                int bR = this._RequestInput.Read(buffer, 0, buffer.Length);

                if (bR == 0)
                    break;

                content += Encoding.ASCII.GetString(buffer);

                NL = 4;
                int EOFIndex = content.IndexOf(RNRN);
                if (EOFIndex == -1)
                {
                    NL = 2;
                    EOFIndex = content.IndexOf(NN);
                }
                if (EOFIndex == -1)
                    continue;

                EOFIndex += NL;

                this._RequestInput.Seek(EOFIndex, SeekOrigin.Begin);

                return content.Substring(0, EOFIndex);
            } while (true);

            return string.Empty;
        }

        private void Parse()
        {
            string header = this.ExtractHeader();

            StringReader sR = new StringReader(header);

            int lineNumber = 1;
            while (sR.Peek() > -1)
            {
                string line = sR.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    this._EOF = true;

                    break;
                }

                switch (lineNumber)
                {
                    case 1:
                        string[] lineParts = line.Split(' ');

                        if (!System.Enum.TryParse<Basics.Context.HttpMethod>(lineParts[0], out this._Method))
                            this._Method = Basics.Context.HttpMethod.GET;

                        this._URL = new URL(lineParts[1]);
                        this._Protocol = lineParts[2];

                        break;

                    default:
                        int colonIndex = line.IndexOf(':');
                        if (colonIndex == -1)
                            return;

                        string key = line.Substring(0, colonIndex);
                        string value = line.Substring(colonIndex + 1);
                        value = value.Trim();

                        switch (key.ToLowerInvariant())
                        {
                            case "host":
                                this._Host = value;

                                break;
                            case "user-agent":
                                this._UserAgent = value;

                                break;
                            case "content-length":
                                int.TryParse(value, out this._ContentLength);

                                break;
                            case "content-type":
                                string[] contentTypeValues = value.Split(';');

                                this._ContentType = contentTypeValues[0];

                                for (int cC = 1; cC < contentTypeValues.Length; cC++)
                                { 
                                    string keyAndValue = contentTypeValues[cC];

                                    int equalsIndex = keyAndValue.IndexOf('=');
                                    if (equalsIndex == -1)
                                        continue;

                                    string contentKey =
                                        keyAndValue.Substring(0, equalsIndex).Trim();
                                    switch(contentKey)
                                    {
                                        case "boundary":
                                            string boundaryValue =
                                                keyAndValue.Substring(equalsIndex + 1).Trim();
                                            this._Boundary = boundaryValue.Replace("\"", string.Empty);

                                            break;
                                        case "charset":
                                            string charsetValue =
                                                keyAndValue.Substring(equalsIndex + 1).Trim();
                                            try
                                            {
                                                this._ContentEncoding = Encoding.GetEncoding(charsetValue);
                                            }
                                            catch (System.Exception)
                                            {
                                                this._ContentEncoding = null;
                                            }

                                            break;
                                    }
                                }

                                break;
                            case "cookie":
                                this.ParseCookies(value);

                                break;
                            default:
                                base.AddOrUpdate(key, value);

                                break;
                        }

                        break;
                }

                lineNumber++;
            }
        }

        private void ParseCookies(string rawCookie)
        {
            string[] keyValues = rawCookie.Split(';');

            foreach (string keyValue in keyValues)
            {
                int equalsIndex = keyValue.IndexOf('=');
                if (equalsIndex == -1)
                    continue;

                string key = keyValue.Substring(0, equalsIndex);
                key = key.Trim();
                string value = keyValue.Substring(equalsIndex + 1);
                value = value.Trim();

                Basics.Context.IHttpCookieInfo cookieInfo = this._Cookie.CreateNewCookie(key);
                cookieInfo.Value = System.Web.HttpUtility.UrlDecode(value);

                this._Cookie.AddOrUpdate(cookieInfo);
            }
        }

        public Basics.Context.HttpMethod Method => this._Method;
        public Basics.Context.IURL URL => this._URL;
        public string Protocol => this._Protocol;

        public string Host => this._Host;
        public string UserAgent => this._UserAgent;
        public int ContentLength => this._ContentLength;
        public string ContentType => this._ContentType;
        public Encoding ContentEncoding => this._ContentEncoding;
        public string Boundary => this._Boundary;

        public Basics.Context.IHttpCookie Cookie => this._Cookie;

        internal bool EOF => this._EOF;

        internal void ReplacePath(string rawURL)
        {
            this._URL = new URL(rawURL);
        }
    }
}
