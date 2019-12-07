using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class HashCodePointedTemplate : Directive
    {
        private readonly string _TemplateId;

        public HashCodePointedTemplate(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.HashCodePointedTemplate, arguments) =>
            this._TemplateId = DirectiveHelper.CaptureDirectiveId(rawValue);

        public override bool Searchable => false;
        public override bool CanAsync => true;
        public override bool CanHoldVariable => false;

        public override void Parse() =>
            this.Children = new DirectiveCollection(this.Mother, this);

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            this.Children.Add(
                new Static($"{Helpers.Context.HashCode}/{this._TemplateId}"));
            return true;
        }

        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, this.Result);
    }
}