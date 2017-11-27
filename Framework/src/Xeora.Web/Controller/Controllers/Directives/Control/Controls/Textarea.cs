namespace Xeora.Web.Controller.Directive.Control
{
    public class Textarea : Control, IHasContent
    {
        public Textarea(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        {
            this.Content = settings.Content;
        }

        public string Content { get; private set; }

        public override IControl Clone() =>
            new Textarea(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            // Textarea Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            // Render Text Content
            this.Content = ControllerHelper.RenderSingleContent(this.Content, this, this.ContentArguments, requesterUniqueID);

            // NO BIND REQUIRED FOR TEXTAREA CONTROL

            // Render Attributes
            for (int aC = 0; aC < this.Attributes.Count; aC++)
            {
                AttributeInfo item = this.Attributes[aC];

                this.Attributes[aC] =
                    new AttributeInfo(
                        item.Key,
                        ControllerHelper.RenderSingleContent(item.Value, this, this.ContentArguments, requesterUniqueID)
                    );
            }
            // !--

            if (this.Security.Disabled.IsSet && this.Security.Disabled.Type == SecurityInfo.DisabledClass.DisabledTypes.Dynamic)
                this.RenderedValue = this.Security.Disabled.Value;
            else
            {
                this.RenderedValue =
                    string.Format(
                        "<textarea name=\"{0}\" id=\"{0}\"{1}>{2}</textarea>",
                        this.ControlID, this.Attributes.ToString(), this.Content
                    );
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }
    }
}