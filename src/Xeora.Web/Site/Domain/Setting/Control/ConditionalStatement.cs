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
            Bind bind = null;

            if (base.Bind != null)
                base.Bind.Clone(out bind);

            SecurityDefinition security = null;

            if (base.Security != null)
                base.Security.Clone(out security);

            return new ConditionalStatement(bind, security);
        }
    }
}