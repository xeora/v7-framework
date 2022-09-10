using System.Net;
using Xeora.Web.Service.Context.Request;
using HttpRequestHeader = Xeora.Web.Service.Context.Request.HttpRequestHeader;

namespace Xeora.Web.Service.Context
{
    public class HttpRequest : Basics.Context.IHttpRequest
    {
        private HttpRequestHeader _Header;

        public HttpRequest(IPAddress remoteAddr) =>
            this.RemoteAddr = remoteAddr;

        public IPAddress RemoteAddr { get; }
        public Basics.Context.Request.IHttpRequestHeader Header => this._Header;
        public Basics.Context.Request.IHttpRequestQueryString QueryString { get; private set; }
        public Basics.Context.Request.IHttpRequestBody Body { get; private set; }

        public ParserResultTypes Build(string contextId, Net.NetworkStream streamEnclosure)
        {
            this._Header = new HttpRequestHeader(streamEnclosure);

            ParserResultTypes parserResult = 
                this._Header.Parse();
            if (parserResult != ParserResultTypes.Success) return parserResult;
            
            this.QueryString = new HttpRequestQueryString(this._Header.Url);

            if (this._Header.WebSocket)
                return ParserResultTypes.Success;
            
            this.Body = new HttpRequestBody(contextId, this._Header, streamEnclosure);

            return ((HttpRequestBody)this.Body).Parse();
        }

        public void RewritePath(string rawUrl)
        {
            ((HttpRequestHeader)this.Header).ReplacePath(rawUrl);
            this.QueryString = new HttpRequestQueryString(this.Header.Url);
        }

        public ParserResultTypes ExportAsWebSocket(out WebSocketRequest webSocketRequest)
        {
            webSocketRequest = null;

            WebSocketRequestHeader webSocketRequestHeader =
                new WebSocketRequestHeader(this._Header);
            ParserResultTypes result =
                webSocketRequestHeader.Ensure();
            if (result != ParserResultTypes.Success) return result;

            webSocketRequest = new WebSocketRequest(this.RemoteAddr, webSocketRequestHeader, this.QueryString);

            return ParserResultTypes.Success;
        }
        
        public void Conclude() => 
            ((HttpRequestBody)this.Body).Conclude();
        
        public void Dispose() =>
            ((HttpRequestBody)this.Body).Dispose();
    }
}
