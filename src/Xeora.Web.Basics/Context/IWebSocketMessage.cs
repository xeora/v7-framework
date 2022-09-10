namespace Xeora.Web.Basics.Context
{
    public interface IWebSocketMessage
    {
        WebSocketMessageTypes Type { get; }
        long Length { get; }
        
        string AsText();
        byte[] AsBinary();
    }
}
