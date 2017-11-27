using System;

namespace Xeora.Web.Controller.Directive.Control
{
    public class Unknown : Control
    {
        public Unknown(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) : 
            base(rawStartIndex, rawValue, contentArguments, settings)
        { }

        public override IControl Clone() =>
            new Unknown(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID) =>
            throw new NotSupportedException(string.Format("Unknown Custom Control Type! Raw: {0}", this.RawValue));
    }
}