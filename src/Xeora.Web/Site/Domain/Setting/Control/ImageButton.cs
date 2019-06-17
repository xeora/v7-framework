using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class ImageButton : Base, IImageButton
    {
        public ImageButton(Bind bind, SecurityDefinition security, string source, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.ImageButton, bind, security)
        {
            this.Source = source;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Source { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            base.Bind.Clone(out Bind bind);

            return new ImageButton(bind, base.Security, this.Source, this.Updates, this.Attributes);
        }
    }
}