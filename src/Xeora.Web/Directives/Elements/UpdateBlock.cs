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
            this.DirectiveID = DirectiveHelper.CaptureDirectiveID(rawValue);
            this._Contents = new ContentDescription(rawValue);
        }

        public string DirectiveID { get; private set; }

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

            if (blockContent.IndexOf(RENDER_ON_REQUEST_MARKER) == 0)
            {
                if (!this.Mother.UpdateBlockIDStack.Contains(this.DirectiveID))
                {
                    this._RenderOnRequest = true;

                    return;
                }

                blockContent = blockContent.Remove(0, RENDER_ON_REQUEST_MARKER.Length);
            }

            this.Mother.RequestParsing(blockContent, ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            if (!this._RenderOnRequest)
            {
                if (this.Mother.UpdateBlockIDStack.Count > 0)
                    this.Mother.UpdateBlockIDStack.Push(this.DirectiveID);
                try
                {
                    this.Children.Render(this.UniqueID);
                }
                finally
                {
                    if (this.Mother.UpdateBlockIDStack.Count > 0)
                        this.Mother.UpdateBlockIDStack.Pop();
                }
            }

            this.Deliver(RenderStatus.Rendered, string.Format("<div id=\"{0}\">{1}</div>", this.DirectiveID, this.Result));
        }
    }
}