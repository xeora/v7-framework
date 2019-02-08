using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Controller.Directive
{
    public class FormattableTranslation : DirectiveWithChildren, IInstanceRequires, INamable
    {
        public event InstanceHandler InstanceRequested;

        public FormattableTranslation(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.FormattableTranslation, contentArguments)
        {
            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
        }

        public string ControlID { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            // FormattableTranslation does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            string blockContent = contentDescription.Parts[0];

            this.Parse(blockContent);

            if (string.IsNullOrEmpty(blockContent))
                throw new Exception.EmptyBlockException();

            base.Render(requesterUniqueID);
        }

        private static Regex _FormatIndexRegEx =
            new Regex("\\{(?<index>\\d+)\\}", RegexOptions.Compiled);
        public override void Build()
        {
            base.Build();

            IDomain instance = null;
            InstanceRequested?.Invoke(ref instance);

            string translationValue = 
                instance.Languages.Current.Get(this.ControlID);
            string[] parameters = this.RenderedValue.Split('|');

            MatchCollection matches =
                FormattableTranslation._FormatIndexRegEx.Matches(translationValue);

            for (int c = matches.Count - 1; c >= 0; c--)
            {
                Match current = matches[c];
                int formatIndex = 
                    int.Parse(current.Groups["index"].Value);

                if (formatIndex >= parameters.Length)
                    throw new Exception.FormatIndexOutOfRangeException();

                translationValue = 
                    translationValue.Remove(current.Index, current.Length);
                translationValue = 
                    translationValue.Insert(current.Index, parameters[formatIndex]);
            }

            this.RenderedValue = translationValue;
        }
    }
}
