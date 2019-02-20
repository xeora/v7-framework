using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface IBase
    {
        ControlTypes Type { get; }
        Bind Bind { get; }
        SecurityDefinition Security { get; }
        IBase Clone();
    }
}