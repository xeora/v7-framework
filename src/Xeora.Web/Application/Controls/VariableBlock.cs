using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Application.Controls
{
    public class VariableBlock : Base, IVariableBlock
    {
        public VariableBlock(Bind bind, SecurityDefinition security) :
            base(ControlTypes.VariableBlock, bind, security)
        { }

        public override IBase Clone()
        {
            Bind bind = null;

            if (base.Bind != null)
                base.Bind.Clone(out bind);

            SecurityDefinition security = null;

            if (base.Security != null)
                base.Security.Clone(out security);

            return new VariableBlock(bind, security);
        }
    }
}