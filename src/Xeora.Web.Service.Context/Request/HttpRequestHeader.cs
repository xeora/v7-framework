using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Service.Context
{
    public class HttpRequestHeader : KeyValueCollection<string, string>, Basics.Context.IHttpRequestHeader
    {
        private readonly Net.NetworkStream _StreamEnclosure;

        private Basics.Context.HttpMethod _Method;
        private int _ContentLength = 0;

        public HttpRequestHeader(Net.NetworkStream streamEnclosure) : 
            base(StringComparer.OrdinalIgnoreCase)
        {
            this._StreamEnclosure = streamEnclosure;

            this.Cookie = new HttpCookie();
        }

        public bool Parse()
        {
            string header = this.ExtractHeader();

            StringReader sR = new StringReader(header);

            int lineNumber = 1;
            while (sR.Peek() > -1)
            {
                string line = sR.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    if (lineNumber == 1)
                        return false;

                    return true;
                }

                switch (lineNumber)
                {
                    case 1:
                        string[] lineParts = line.Split(' ');

                        if (!Enum.TryParse<Basics.Context.HttpMethod>(lineParts[0], out this._Method))
                            this._Method = Basics.Context.HttpMethod.GET;

                        this.URL = new URL(lineParts[1]);
                        this.Protocol = lineParts[2];

                        break;

                    default:
                        int colonIndex = line.IndexOf(':');
                        if (colonIndex == -1)
                            return false;

                        string key = line.Substring(0, colonIndex);
                        string value = line.Substring(colonIndex + 1);
                        value = value.Trim();

                        switch (key.ToLowerInvariant())
                        {
                            case "host":
                                this.Host = value;

                                break;
                            case "user-agent":
                                this.UserAgent = value;

                                break;
                            case "content-length":
                                int.TryParse(value, out this._ContentLength);

                                break;
                            case "content-type":
                                string[] contentTypeValues = value.Split(';');

                                this.ContentType = contentTypeValues[0];

                                for (int cC = 1; cC < contentTypeValues.Length; cC++)
                                {
                                    string keyAndValue = contentTypeValues[cC];

                                    int equalsIndex = keyAndValue.IndexOf('=');
                                    if (equalsIndex == -1)
                                        continue;

                                    string contentKey =
                                        keyAndValue.Substring(0, equalsIndex).Trim();
                                    switch (contentKey)
                                    {
                                        case "boundary":
                                            string boundaryValue =
                                                keyAndValue.Substring(equalsIndex + 1).Trim();
                                            this.Boundary = boundaryValue.Replace("\"", string.Empty);

                                            break;
                                        case "charset":
                                            string charsetValue =
                                                keyAndValue.Substring(equalsIndex + 1).Trim();
                                            try
                                            {
                                                this.ContentEncoding = Encoding.GetEncoding(charsetValue);
                                            }
                                            catch (Exception)
                                            {
                                                this.ContentEncoding = null;
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

            return false;
        }

        private string ExtractHeader()
        {
            string RNRN = "\r\n\r\n";
            string NN = "\n\n";
            int NL;

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();

                string content = string.Empty;
                int EOFIndex = 0;

                bool completed = this._StreamEnclosure.Listen((buffer, size) =>
                {
                    contentStream.Write(buffer, 0, size);
                    content += Encoding.ASCII.GetString(buffer, 0, size);

                    NL = 4;
                    EOFIndex = content.IndexOf(RNRN);
                    if (EOFIndex == -1)
                    {
                        NL = 2;
                        EOFIndex = content.IndexOf(NN);
                    }
                    if (EOFIndex == -1)
                        return true;

                    EOFIndex += NL;

                    byte[] residualData = new byte[content.Length - EOFIndex];
                    contentStream.Seek(EOFIndex, SeekOrigin.Begin);
                    contentStream.Read(residualData, 0, residualData.Length);

                    this._StreamEnclosure.Return(residualData, 0, residualData.Length);

                    return false;
                });

                if (!completed)
                    return string.Empty;

                return content.Substring(0, EOFIndex);
            }
            finally
            {
                contentStream?.Close();
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

                Basics.Context.IHttpCookieInfo cookieInfo = this.Cookie.CreateNewCookie(key);
                cookieInfo.Value = System.Web.HttpUtility.UrlDecode(value);

                this.Cookie.AddOrUpdate(cookieInfo);
            }
        }

        public Basics.Context.HttpMethod Method => this._Method;
        public Basics.Context.IURL URL { get; private set; }
        public string Protocol { get; private set; }

        public string Host { get; private set; }
        public string UserAgent { get; private set; }
        public int ContentLength => this._ContentLength;
        public string ContentType { get; private set; }
        public Encoding ContentEncoding { get; private set; }
        public string Boundary { get; private set; }

        public Basics.Context.IHttpCookie Cookie { get; private set; }

        internal void ReplacePath(string rawURL) =>
            this.URL = new URL(rawURL);
    }
}
