using Xeora.Web.Basics;

namespace Xeora.Web.Handler
{
    internal class HandlerContainer
    {
        public HandlerContainer(ref IHandler handler)
        {
            this.Handler = handler;
            this.Removable = true;
        }

        public IHandler Handler { get; private set; }
        public bool Removable { get; set; }
    }
}
