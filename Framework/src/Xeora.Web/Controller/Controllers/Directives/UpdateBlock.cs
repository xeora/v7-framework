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
            if (this.IsUpdateBlockController)
                throw new Exception.RequestBlockException();

            Global.ContentDescription contentDescription = 
                new Global.ContentDescription(this.Value);

            string blockContent = contentDescription.Parts[0];
            string renderOnRequestMarker = "!RENDERONREQUEST";

            if (blockContent.IndexOf(renderOnRequestMarker) == 0)
            {
                if (string.Compare(this.ControlID, this.Mother.ProcessingUpdateBlockControlID) != 0)
                {
                    this.RenderedValue = string.Format("<div id=\"{0}\"></div>", this.ControlID);

                    return;
                }

                blockContent = blockContent.Replace(renderOnRequestMarker, string.Empty);
            }

            this.Parse(blockContent);
            base.Render(requesterUniqueID);
        }

        public override void Build()
        {
            base.Build();

            this.RenderedValue = string.Format("<div id=\"{0}\">{1}</div>", this.ControlID, this.RenderedValue);
        }
    }
}