using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Single : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public Single(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Single, arguments)
        {
            rawValue = $"Single~0:{{{rawValue}}}:Single~0";

            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => true;
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            this.Mother.RequestParsing(this._Contents.Parts[0], ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Children.Render(this.UniqueId);
            this.Deliver(RenderStatus.Rendered, this.Result);
        }
    }
}