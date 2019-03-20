using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class HashCodePointedTemplate : Directive
    {
        private readonly string _TemplateID;

        public HashCodePointedTemplate(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.HashCodePointedTemplate, arguments)
        {
            this._TemplateID = DirectiveHelper.CaptureDirectiveID(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => true;

        public override void Parse()
        { }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Deliver(
                RenderStatus.Rendered,
                string.Format("{0}/{1}", Helpers.Context.HashCode, this._TemplateID)
            );
        }
    }
}