using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class MessageBlock : Directive
    {
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public MessageBlock(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.MessageBlock, arguments) =>
            this._Contents = new ContentDescription(rawValue);

        public override bool Searchable => false;
        public override bool CanAsync => true;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            if (this.Mother.MessageResult == null)
                return;

            this.Arguments.AppendKeyWithValue("MessageType", this.Mother.MessageResult.Type);
            this.Arguments.AppendKeyWithValue("Message", this.Mother.MessageResult.Content);

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(this._Contents.Parts[0], this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            return this.Mother.MessageResult != null;
        }

        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, this.Result);
    }
}