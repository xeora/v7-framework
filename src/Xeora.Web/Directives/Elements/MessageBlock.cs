using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class MessageBlock : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public MessageBlock(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.MessageBlock, arguments)
        {
            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => true;

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

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            if (this.Mother.MessageResult != null)
                this.Children.Render(this.UniqueId);

            this.Deliver(RenderStatus.Rendered, this.Result);
        }
    }
}