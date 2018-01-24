using Xeora.Web.Basics.Context;

namespace Xeora.Web.Basics
{
    public interface IHandler
    {
        string HandlerID { get; }
        IHttpContext Context { get; }
        IDomainControl DomainControl { get; }
    }
}
