using System.Text.RegularExpressions;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class ReplaceableTranslation : Directive, INameable
    {
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public ReplaceableTranslation(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.ReplaceableTranslation, arguments)
        {
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this._Contents = new ContentDescription(rawValue);
        }

        public string DirectiveId { get; }

        public override bool Searchable => false;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => false;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;
            
            // ReplaceableTranslation needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            string formatContent = this._Contents.Parts[0];
            if (string.IsNullOrEmpty(formatContent))
                throw new Exceptions.EmptyBlockException();
            
            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(formatContent, this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            return true;
        }

        private static readonly Regex FormatIndexRegEx =
            new Regex("\\{(?<index>\\d+)\\}", RegexOptions.Compiled);
        public override void PostRender()
        {
            this.Mother.RequestInstance(out IDomain instance);

            string translationValue =
                instance.Languages.Current.Get(this.DirectiveId);
            string[] parameters = this.Result.Split('|');

            MatchCollection matches =
                ReplaceableTranslation.FormatIndexRegEx.Matches(translationValue);

            for (int c = matches.Count - 1; c >= 0; c--)
            {
                Match current = matches[c];
                int formatIndex =
                    int.Parse(current.Groups["index"].Value);

                if (formatIndex >= parameters.Length)
                    throw new Exceptions.FormatIndexOutOfRangeException(DirectiveTypes.ReplaceableTranslation.ToString());

                translationValue =
                    translationValue.Remove(current.Index, current.Length);
                translationValue =
                    translationValue.Insert(current.Index, parameters[formatIndex]);
            }

            this.Deliver(RenderStatus.Rendered, translationValue);
            this.Scheduler.Fire();
        }
    }
}
