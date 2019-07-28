using System;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class Message
    {
        public enum Types
        {
            Error,
            Warning,
            Success
        }

        public Message(string content, Types type = Types.Error)
        {
            this.Content = content;
            this.Type = type;
        }

        public string Content { get; }
        public Types Type { get; }
    }
}