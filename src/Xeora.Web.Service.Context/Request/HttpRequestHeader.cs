using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestHeader : KeyValueCollection<string, string>, Basics.Context.Request.IHttpRequestHeader
    {
        private static string[] _SupportedHttpProtocols = { "HTTP/1.0", "HTTP/1.1" };
        private readonly Net.NetworkStream _StreamEnclosure;

        private Basics.Context.Request.HttpMethod _Method;
        private int _ContentLength;

        public HttpRequestHeader(Net.NetworkStream streamEnclosure) : 
            base(StringComparer.OrdinalIgnoreCase)
        {
            this._StreamEnclosure = streamEnclosure;

            this.Cookie = new HttpCookie();
        }

        public ParserResultTypes Parse()
        {
            string header = this.ExtractHeader();
            if (string.IsNullOrEmpty(header)) return ParserResultTypes.Timeout;

            StringReader sR = new StringReader(header);

            int lineNumber = 1;
            while (sR.Peek() > -1)
            {
                string line = sR.ReadLine();

                if (string.IsNullOrEmpty(line))
                    return lineNumber != 1 ? ParserResultTypes.Success : ParserResultTypes.BadRequest;

                switch (lineNumber)
                {
                    case 1:
                        string[] lineParts = line.Split(' ');

                        if (!Enum.TryParse(lineParts[0], out this._Method))
                            return ParserResultTypes.MethodNotAllowed;

                        this.Url = new Url(lineParts[1]);
                        this.Protocol = lineParts[2];

                        if (Array.IndexOf(HttpRequestHeader._SupportedHttpProtocols, this.Protocol) == -1)
                            return ParserResultTypes.HttpVersionNotSupported;

                        break;
                    default:
                        int colonIndex = line.IndexOf(':');
                        if (colonIndex == -1)
                            return ParserResultTypes.BadRequest;

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
                            case "upgrade":
                                this.WebSocket = string.CompareOrdinal(value, "websocket") == 0;
                                break;
                            default:
                                AddOrUpdate(key, value);

                                break;
                        }

                        break;
                }

                lineNumber++;
            }

            return ParserResultTypes.BadRequest;
        }

        private string ExtractHeader()
        {
            const string rnrn = "\r\n\r\n";
            const string nn = "\n\n";
            int nl;

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();

                string content = string.Empty;
                int eofIndex = 0;

                bool completed = this._StreamEnclosure.Listen((buffer, size) =>
                {
                    contentStream.Write(buffer, 0, size);
                    content += Encoding.ASCII.GetString(buffer, 0, size);

                    nl = 4;
                    eofIndex = 
                        content.IndexOf(rnrn, StringComparison.Ordinal);
                    if (eofIndex == -1)
                    {
                        nl = 2;
                        eofIndex = content.IndexOf(nn, StringComparison.Ordinal);
                    }
                    if (eofIndex == -1)
                        return true;

                    eofIndex += nl;

                    byte[] residualData = new byte[content.Length - eofIndex];
                    contentStream.Seek(eofIndex, SeekOrigin.Begin);
                    contentStream.Read(residualData, 0, residualData.Length);

                    this._StreamEnclosure.Return(residualData, 0, residualData.Length);

                    return false;
                });

                return !completed ? string.Empty : content.Substring(0, eofIndex);
            }
            finally
            {
                contentStream?.Dispose();
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

        public Basics.Context.Request.HttpMethod Method => this._Method;
        public Basics.Context.IUrl Url { get; private set; }
        public string Protocol { get; private set; }

        public string Host { get; private set; }
        public string UserAgent { get; private set; }
        public int ContentLength => this._ContentLength;
        public string ContentType { get; private set; }
        public Encoding ContentEncoding { get; private set; }
        public string Boundary { get; private set; }
        public Basics.Context.IHttpCookie Cookie { get; }

        public bool KeepAlive => 
            !string.IsNullOrEmpty(this["Connection"]) && this["Connection"].IndexOf("keep-alive", StringComparison.Ordinal) > -1;
        public bool WebSocket { get; private set; }

        internal void ReplacePath(string rawUrl) =>
            this.Url = new Url(rawUrl);
    }
}
