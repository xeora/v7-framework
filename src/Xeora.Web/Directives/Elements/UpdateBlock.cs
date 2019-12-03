using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class UpdateBlock : Directive, INameable
    {
        private const string RENDER_ON_REQUEST_MARKER = "!RENDERONREQUEST";

        private bool _RenderOnRequest;
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public UpdateBlock(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.UpdateBlock, arguments)
        {
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this._Contents = new ContentDescription(rawValue);
            
            this.UpdateBlockIds.Add(this.DirectiveId);
        }

        public string DirectiveId { get; }

        public override bool Searchable => true;
        public override bool CanAsync => false;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            string blockContent = this._Contents.Parts[0];

            if (blockContent.IndexOf(RENDER_ON_REQUEST_MARKER, System.StringComparison.InvariantCulture) == 0)
            {
                if (!this.Mother.RequestedUpdateBlockIds.Contains(this.DirectiveId))
                {
                    this._RenderOnRequest = true;
                    return;
                }

                blockContent = blockContent.Remove(0, RENDER_ON_REQUEST_MARKER.Length);
            }

            // UpdateBlock needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(blockContent, this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            if (this._RenderOnRequest)
                this.Deliver(RenderStatus.Rendered, $"<div id=\"{this.DirectiveId}\"></div>");
            
            return !this._RenderOnRequest;
        }
        
        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, $"<div id=\"{this.DirectiveId}\">{this.Result}</div>");
    }
}