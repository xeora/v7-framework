using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class AsyncGroup : Directive
    {
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public AsyncGroup(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.AsyncGroup, arguments) =>
            this._Contents = new ContentDescription(rawValue);

        public override bool Searchable => true;
        public override bool CanAsync => true;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;
            
            // AsyncGroup needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(this._Contents.Parts[0], this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            return true;
        }

        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, this.Result);
    }
}