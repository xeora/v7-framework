using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Application.Controls
{
    public class LinkButton : Base, ILinkButton
    {
        public LinkButton(Bind bind, SecurityDefinition security, string text, string url, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.LinkButton, bind, security)
        {
            this.Text = text;
            this.Url = url;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Text { get; }
        public string Url { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            Bind bind = null;
            Bind?.Clone(out bind);

            SecurityDefinition security = null;
            Security?.Clone(out security);

            return new LinkButton(bind, security, this.Text, this.Url, this.Updates.Clone(), this.Attributes.Clone());
        }
    }
}