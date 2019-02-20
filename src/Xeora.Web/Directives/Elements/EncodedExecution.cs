using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class EncodedExecution : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;
        private bool _Rendered;

        public EncodedExecution(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.EncodedExecution, arguments)
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

            this._Children = new DirectiveCollection(this.Mother, this);

            // EncodedExecution does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            string executionContent = this._Contents.Parts[0];
            if (string.IsNullOrEmpty(executionContent))
                throw new Exception.EmptyBlockException();

            this.Mother.RequestParsing(executionContent, ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this._Rendered)
                return;
            this._Rendered = true;

            this.Children.Render(this.UniqueID);

            if (this.Mother.UpdateBlockIDStack.Count > 0)
            {
                this.Result =
                    string.Format(
                        "javascript:__XeoraJS.update('{0}', '{1}');",
                        this.Mother.UpdateBlockIDStack.Peek(),
                        Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.Result)
                    );

                return;
            }

            this.Result =
                string.Format(
                    "javascript:__XeoraJS.post('{0}');",
                    Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.Result)
                );
        }
    }
}
