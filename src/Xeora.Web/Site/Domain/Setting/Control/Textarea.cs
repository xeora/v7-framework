using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class Textarea : Base
    {
        public Textarea(Bind bind, SecurityDefinition security, string content, AttributeCollection attributes) :
            base(ControlTypes.Textarea, bind, security)
        {
            this.Content = content;
            this.Attributes = attributes;
        }

        public string Content { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            Bind bind = null;

            if (base.Bind != null)
                base.Bind.Clone(out bind);

            SecurityDefinition security = null;

            if (base.Security != null)
                base.Security.Clone(out security);

            return new Textarea(bind, security, this.Content, this.Attributes.Clone());
        }
    }
}