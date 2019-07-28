using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Translation : Directive
    {
        private readonly string _TranslationId;

        public Translation(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Translation, arguments)
        {
            this._TranslationId = DirectiveHelper.CaptureDirectiveId(rawValue);
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

            this.Mother.RequestInstance(out IDomain instance);

            this.Deliver(RenderStatus.Rendered, instance.Languages.Current.Get(this._TranslationId));
        }

    }
}