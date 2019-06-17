using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class ConditionalStatement : Base, IConditionalStatement
    {
        public ConditionalStatement(Bind bind, SecurityDefinition security) :
            base(ControlTypes.ConditionalStatement, bind, security)
        { }

        public override IBase Clone()
        {
            base.Bind.Clone(out Bind bind);

            return new ConditionalStatement(bind, base.Security);
        }
    }
}