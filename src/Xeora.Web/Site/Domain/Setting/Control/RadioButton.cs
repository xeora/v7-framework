using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class RadioButton : Base, IRadioButton
    {
        public RadioButton(Bind bind, SecurityDefinition security, string text, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.RadioButton, bind, security)
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

            return new RadioButton(bind, base.Security, this.Text, this.Updates, this.Attributes);
        }
    }
}