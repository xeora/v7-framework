using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Translation : Directive
    {
        private readonly string _TranslationID;
        private bool _Rendered;

        public Translation(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Translation, arguments)
        {
            this._TranslationID = DirectiveHelper.CaptureDirectiveID(rawValue);
        }

        public override bool Searchable => false;
        public override bool Rendered => this._Rendered;

        public override void Parse()
        { }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this._Rendered)
                return;
            this._Rendered = true;

            IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            this.Result = instance.Languages.Current.Get(this._TranslationID);
        }

    }
}