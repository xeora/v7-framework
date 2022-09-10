namespace Xeora.Web.Basics.Context
{
    public interface IWebSocketContext
    {
        string UniqueId { get; }
        IWebSocketRequest Request { get; }

        delegate void OnOpenedHandler();
        delegate void OnErrorHandler(short errorCode, string errorMessage);
        delegate void OnMessageHandler(IWebSocketMessage message);
        delegate void OnClosedHandler(short statusCode);
        
        event OnOpenedHandler OnOpened;
        event OnErrorHandler OnError;
        event OnMessageHandler OnMessage;
        event OnClosedHandler OnClosed;

        public void Send(string message);
        public void Send(byte[] buffer, int offset, int count);
    }
}
