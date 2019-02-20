using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;

namespace Xeora.Web.Site.Setting.Control
{
    public class Unknown : Base, IUnknown
    {
        public Unknown() :
            base(ControlTypes.Unknown, null, null)
        { }

        public override IBase Clone() =>
            throw new System.Exception("This control cannot be cloned");
    }
}