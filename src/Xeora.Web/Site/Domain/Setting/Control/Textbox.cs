using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class Textbox : Base, ITextbox
    {
        public Textbox(Bind bind, SecurityDefinition security, string text, string defaultButtonID, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.Textbox, bind, security)
        {
            this.Text = text;
            this.DefaultButtonID = defaultButtonID;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Text { get; }
        public string DefaultButtonID { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone() =>
            new Textbox(base.Bind, base.Security, this.Text, this.DefaultButtonID, this.Updates, this.Attributes);
    }
}