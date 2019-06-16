using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class AsyncGroup : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public AsyncGroup(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.AsyncGroup, arguments)
        {
            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => true;
        public override bool CanAsync => true;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            // AsyncGroup needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Mother.RequestParsing(this._Contents.Parts[0], ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Children.Render(this.UniqueID);
            this.Deliver(RenderStatus.Rendered, this.Result);
        }
    }
}