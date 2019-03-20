using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Translation : Directive
    {
        private readonly string _TranslationID;

        public Translation(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Translation, arguments)
        {
            this._TranslationID = DirectiveHelper.CaptureDirectiveID(rawValue);
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

            IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            this.Deliver(RenderStatus.Rendered, instance.Languages.Current.Get(this._TranslationID));
        }

    }
}