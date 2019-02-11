namespace Xeora.Web.Controller.Directive
{
    public class UpdateBlock : DirectiveWithChildren, INamable
    {
        public UpdateBlock(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) : 
            base(rawStartIndex, rawValue, DirectiveTypes.UpdateBlock, contentArguments)
        {
            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
        }

        public string ControlID { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription = 
                new Global.ContentDescription(this.Value);

            string blockContent = contentDescription.Parts[0];
            string renderOnRequestMarker = "!RENDERONREQUEST";

            if (blockContent.IndexOf(renderOnRequestMarker) == 0)
            {
                if (!this.Mother.UpdateBlockControlIDStack.Contains(this.ControlID))
                    return;

                blockContent = blockContent.Remove(0, renderOnRequestMarker.Length);
            }

            this.Parse(blockContent);

            if (this.Mother.UpdateBlockControlIDStack.Count > 0)
                this.Mother.UpdateBlockControlIDStack.Push(this.ControlID);
            try
            {
                base.Render(requesterUniqueID);
            }
            finally
            {
                if (this.Mother.UpdateBlockControlIDStack.Count > 0)
                    this.Mother.UpdateBlockControlIDStack.Pop();
            }
        }

        public override void Build()
        {
            base.Build();

            this.RenderedValue = string.Format("<div id=\"{0}\">{1}</div>", this.ControlID, this.RenderedValue);
        }
    }
}