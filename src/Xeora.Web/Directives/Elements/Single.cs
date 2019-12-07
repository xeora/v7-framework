using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Single : Directive
    {
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public Single(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Single, arguments)
        {
            rawValue = $"Single~0:{{{rawValue}}}:Single~0";
            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => true;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => true;
        
        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

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