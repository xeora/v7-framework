using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class VariableBlock : Base, IVariableBlock
    {
        public VariableBlock(Bind bind, SecurityDefinition security) :
            base(ControlTypes.VariableBlock, bind, security)
        { }

        public override IBase Clone()
        {
            base.Bind.Clone(out Bind bind);

            return new VariableBlock(bind, base.Security);
        }
    }
}