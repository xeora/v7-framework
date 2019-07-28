using Xeora.Web.Basics;

namespace Xeora.Web.Handler
{
    internal class Container
    {
        public Container(ref IHandler handler)
        {
            this.Handler = handler;
            this.Removable = true;
        }

        public IHandler Handler { get; }
        public bool Removable { get; set; }
    }
}
