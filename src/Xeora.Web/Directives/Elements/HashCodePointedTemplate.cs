using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class HashCodePointedTemplate : Directive
    {
        private readonly string _TemplateId;

        public HashCodePointedTemplate(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.HashCodePointedTemplate, arguments)
        {
            this._TemplateId = DirectiveHelper.CaptureDirectiveId(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => true;

        public override void Parse()
        { }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Deliver(
                RenderStatus.Rendered,
                $"{Helpers.Context.HashCode}/{this._TemplateId}"
            );
        }
    }
}