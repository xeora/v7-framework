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

        public override IBase Clone() =>
            new Textarea(base.Bind, base.Security, this.Content, this.Attributes);
    }
}