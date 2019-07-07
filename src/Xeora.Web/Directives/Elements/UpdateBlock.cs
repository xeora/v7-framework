using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class UpdateBlock : Directive, INamable, IHasChildren
    {
        private const string RENDER_ON_REQUEST_MARKER = "!RENDERONREQUEST";

        private bool _RenderOnRequest;
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public UpdateBlock(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.UpdateBlock, arguments)
        {
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this._Contents = new ContentDescription(rawValue);
        }

        public string DirectiveId { get; private set; }

        public override bool Searchable => true;
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            string blockContent = this._Contents.Parts[0];

            if (blockContent.IndexOf(RENDER_ON_REQUEST_MARKER, System.StringComparison.InvariantCulture) == 0)
            {
                if (!this.Mother.UpdateBlockIdStack.Contains(this.DirectiveId))
                {
                    this._RenderOnRequest = true;

                    return;
                }

                blockContent = blockContent.Remove(0, RENDER_ON_REQUEST_MARKER.Length);
            }

            // UpdateBlock needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Mother.RequestParsing(blockContent, ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            if (!this._RenderOnRequest)
            {
                this.Mother.UpdateBlockIdStack.Push(this.DirectiveId);
                try
                {
                    this.Children.Render(this.UniqueId);
                }
                finally
                {
                    this.Mother.UpdateBlockIdStack.Pop();
                }
            }

            this.Deliver(RenderStatus.Rendered, string.Format("<div id=\"{0}\">{1}</div>", this.DirectiveId, this.Result));
        }
    }
}