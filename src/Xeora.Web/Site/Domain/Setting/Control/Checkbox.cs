using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class Checkbox : Base, ICheckbox
    {
        public Checkbox(Bind bind, SecurityDefinition security, string text, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.Checkbox, bind, security)
        {
            this.Text = text;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Text { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            base.Bind.Clone(out Bind bind);

            return new Checkbox(bind, base.Security, this.Text, this.Updates, this.Attributes);
        }
    }
}