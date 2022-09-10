using Xeora.Web.Basics.Context;

namespace Xeora.Web.Service.Context
{
    public class WebSocketMessage : IWebSocketMessage
    {
        private readonly byte[] _Data;
        private readonly string _DataString;
        
        public WebSocketMessage(WebSocketMessageTypes type, byte[] data)
        {
            this.Type = type;
            this._Data = data;
            
            if (type == WebSocketMessageTypes.Text)
                this._DataString = System.Text.Encoding.UTF8.GetString(this._Data);
        }

        public WebSocketMessageTypes Type { get; }
        public long Length => this.Type == WebSocketMessageTypes.Text ? this._DataString.Length : this._Data.Length;
        public string AsText() => this._DataString;
        public byte[] AsBinary() => this._Data;
    }
}
