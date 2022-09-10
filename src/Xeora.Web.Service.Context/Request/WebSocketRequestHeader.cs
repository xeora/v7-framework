using Xeora.Web.Basics.Context;
using Xeora.Web.Basics.Context.Request;

namespace Xeora.Web.Service.Context.Request
{
    public class WebSocketRequestHeader : KeyValueCollection<string, string>, IWebSocketRequestHeader
    {
        internal WebSocketRequestHeader(IHttpRequestHeader header)
        {
            this.Url = header.Url;
            this.Protocol = header.Protocol;

            this.Host = header.Host;
            this.UserAgent = header.UserAgent;

            this.Key = header["sec-websocket-key"] ?? string.Empty;
            this.Extensions = header["sec-websocket-extensions"] ?? string.Empty;
            this.Version = header["sec-websocket-version"] ?? string.Empty;

            foreach (var key in header.Keys)
            {
                if (string.CompareOrdinal(key, "sec-websocket-key") != 0 &&
                    string.CompareOrdinal(key, "sec-websocket-extensions") != 0 &&
                    string.CompareOrdinal(key, "sec-websocket-version") != 0)
                {
                    this.AddOrUpdate(key, header[key]);
                }
            }

            this.Cookie = header.Cookie;
        }
        
        public IUrl Url { get; private set; }
        public string Protocol { get; }
        
        public string Host { get; }
        public string UserAgent { get; }
        
        public string Key { get; set; }
        public string Extensions { get; set; }
        public string Version { get; set; }
        
        public IHttpCookie Cookie { get; }

        public ParserResultTypes Ensure()
        {
            if (string.CompareOrdinal(this.Protocol, "HTTP/1.1") != 0)
                return ParserResultTypes.BadRequest;
            
            if (string.CompareOrdinal(this.Version, "13") != 0)
                return ParserResultTypes.WebSocketVersionNotSupported;

            if (string.IsNullOrEmpty(this.Key))
                return ParserResultTypes.BadRequest;

            return ParserResultTypes.Success;
        }
        
        internal void ReplacePath(string rawUrl) =>
            this.Url = new Url(rawUrl);
    }
}
