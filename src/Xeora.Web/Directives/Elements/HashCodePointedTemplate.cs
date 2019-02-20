using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class HashCodePointedTemplate : Directive
    {
        private readonly string _TemplateID;
        private bool _Rendered;

        public HashCodePointedTemplate(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.HashCodePointedTemplate, arguments)
        {
            this._TemplateID = DirectiveHelper.CaptureDirectiveID(rawValue);
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

            this.Result = string.Format("{0}/{1}", Helpers.Context.HashCode, this._TemplateID);
        }
    }
}