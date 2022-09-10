namespace Xeora.Web.Service.Context.WebSocket
{
    public enum OpCodes
    {
        None = -1,
        Continue = 0,
        Text = 1,
        Binary = 2,
        Close = 8,
        Ping = 9,
        Pong = 10
    }
}