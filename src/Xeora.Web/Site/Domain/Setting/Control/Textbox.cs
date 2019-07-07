using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class Textbox : Base, ITextbox
    {
        public Textbox(Bind bind, SecurityDefinition security, string text, string defaultButtonId, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.Textbox, bind, security)
        {
            this.Text = text;
            this.DefaultButtonId = defaultButtonId;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Text { get; }
        public string DefaultButtonId { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            base.Bind.Clone(out Bind bind);

            return new Textbox(bind, base.Security, this.Text, this.DefaultButtonId, this.Updates, this.Attributes);
        }
    }
}