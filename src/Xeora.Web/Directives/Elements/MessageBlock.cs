using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class MessageBlock : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;
        private bool _Rendered;

        public MessageBlock(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.MessageBlock, arguments)
        {
            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => false;
        public override bool Rendered => this._Rendered;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            if (this.Mother.MessageResult == null)
                return;

            this._Children = new DirectiveCollection(this.Mother, this);

            this.Arguments.AppendKeyWithValue("MessageType", this.Mother.MessageResult.Type);
            this.Arguments.AppendKeyWithValue("Message", this.Mother.MessageResult.Content);

            this.Mother.RequestParsing(this._Contents.Parts[0], ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this._Rendered)
                return;
            this._Rendered = true;

            if (this.Mother.MessageResult != null)
                this.Children.Render(this.UniqueID);
        }
    }
}