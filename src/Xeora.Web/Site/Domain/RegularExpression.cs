using System.Text.RegularExpressions;

namespace Xeora.Web.Site
{
    public class RegularExpression
    {
        private RegularExpression()
        {
            // CAPTURE REGULAR EXPRESSIONS
            string characterGroup = "0-9_a-zA-Z";
            string variableRegEx = "([#]+|[\\^\\-\\+\\*\\~])?([" + characterGroup + "]+)|\\=([\\S+][^\\$\\|]*)|@([#]+|[\\-])?([" + characterGroup + "]+\\.)[\\." + characterGroup + "]+";
            string tagIDRegEx = "[\\.\\-" + characterGroup + "]+";
            string tagIDWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            string directivePointerRegEx = "[A-Z]";
            string levelingRegEx = "\\#\\d+(\\+)?";
            string parentingRegEx = "\\[" + tagIDRegEx + "\\]";
            string procedureRegEx = tagIDRegEx + "\\?" + tagIDRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

            string captureRegEx =
                "\\$" +
                "(" +
                    "(" + variableRegEx + ")\\$" + // Capture Variable
                    "|" +
                    tagIDRegEx + "\\:\\{" + // Capture Special Tag
                    "|" +
                    directivePointerRegEx + "(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:" + // Capture Directive with Leveling and Parenting
                    "(" +
                        tagIDWithSlashRegEx + "\\$" + // Capture Control without content
                        "|" +
                        tagIDRegEx + "\\:\\{" + // Capture Control with content (opening)
                        "|" +
                        procedureRegEx + "\\$" + // Capture Procedure
                    ")" + 
                ")|(" + 
                    "(" +
                        "\\}\\:" + tagIDRegEx +
                        "(" +
                            "\\:\\{" + // Capture Control with Content (seperator)
                            "|" +
                            "\\$" + // Capture Control with Content (closing)
                        ")" +
                    ")" +
                ")"; 
            string bracketedRegExOpening = "\\$((?<ItemID>" + tagIDRegEx + ")|(?<DirectiveType>" + directivePointerRegEx + ")(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:(?<ItemID>" + tagIDRegEx + "))\\:\\{";
            string bracketedRegExSeparator = "\\}:(?<ItemID>" + tagIDRegEx + ")\\:\\{";
            string bracketedRegExClosing = "\\}:(?<ItemID>" + tagIDRegEx + ")\\$";
            // !---

            this.MainCapturePattern =
                new Regex(captureRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerOpenPattern =
                new Regex(bracketedRegExOpening, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerSeparatorPattern =
                new Regex(bracketedRegExSeparator, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerClosePattern =
                new Regex(bracketedRegExClosing, RegexOptions.Multiline | RegexOptions.Compiled);
            this.VariableCapturePattern =
                new Regex(variableRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        private static RegularExpression _Current = null;
        public static RegularExpression Current
        {
            get
            {
                if (RegularExpression._Current == null)
                    RegularExpression._Current = new RegularExpression();

                return RegularExpression._Current;
            }
        }

        public Regex MainCapturePattern { get; private set; }
        public Regex BracketedControllerOpenPattern { get; private set; }
        public Regex BracketedControllerSeparatorPattern { get; private set; }
        public Regex BracketedControllerClosePattern { get; private set; }
        public Regex VariableCapturePattern { get; private set; }
    }
}
