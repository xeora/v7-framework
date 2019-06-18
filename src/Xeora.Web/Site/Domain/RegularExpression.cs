using System.Text.RegularExpressions;
using System.Threading;

namespace Xeora.Web.Site
{
    public class RegularExpression
    {
        private readonly string SpecificContentOpeningRegEx;

        private RegularExpression()
        {
            // CAPTURE REGULAR EXPRESSIONS
            string characterGroup = "0-9_a-zA-Z";
            string simpleVariableRegEx = "([#]+|[\\^\\-\\+\\*\\~])?([" + characterGroup + "]+)";
            string staticVariableRegEx = "\\=[\\S]*";
            string objectVariableRegEx = "@([#]+|[\\-])?([" + characterGroup + "]+\\.)[\\." + characterGroup + "]+";
            string variableRegEx = simpleVariableRegEx + "|" + staticVariableRegEx + "|" + objectVariableRegEx;
            string directiveIDRegEx = "[\\.\\-" + characterGroup + "]+";
            string directiveIDWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            string directivePointerRegEx = "[A-Z]{1,2}";
            string levelingRegEx = "\\#\\d+(\\+)?";
            string parentingRegEx = "\\[" + directiveIDRegEx + "\\]";
            string parametersRegEx = "\\(((\\|)?(" + variableRegEx + ")?)+\\)";
            string procedureRegEx = directiveIDRegEx + "\\?" + directiveIDRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

            string singleRegEx =
                "\\$" +
                "(" +
                    "(" + variableRegEx + ")\\$" + // Capture Variable
                    "|" +
                    directivePointerRegEx + "(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:" + // Capture Directive with Leveling and Parenting
                    "(" +
                        directiveIDWithSlashRegEx + "\\$" + // Capture Control without content
                        "|" +
                        procedureRegEx + "\\$" + // Capture Procedure
                    ")" + 
                ")";
            string contentOpeningRegEx = "\\$((?<DirectiveID>" + directiveIDRegEx + ")|(?<DirectiveType>" + directivePointerRegEx + ")(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:(?<DirectiveID>" + directiveIDRegEx + ")(" + parametersRegEx + ")?)\\:\\{";
            string contentSeparatorRegEx = "\\}:(?<DirectiveID>" + directiveIDRegEx + ")\\:\\{";
            string contentClosingRegEx = "\\}:(?<DirectiveID>" + directiveIDRegEx + ")\\$";

            this.SpecificContentOpeningRegEx = "\\$(({0})|({1})(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:({0}))";
            // !---

            this.SingleCapturePattern =
                new Regex(singleRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
            this.ContentOpeningPattern =
                new Regex(contentOpeningRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
            this.ContentSeparatorPattern =
                new Regex(contentSeparatorRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
            this.ContentClosingPattern =
                new Regex(contentClosingRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        private static object _Lock = new object();
        private static RegularExpression _Current = null;
        public static RegularExpression Current
        {
            get
            {
                Monitor.Enter(RegularExpression._Lock);
                try
                {
                    if (RegularExpression._Current == null)
                        RegularExpression._Current = new RegularExpression();
                }
                finally
                {
                    Monitor.Exit(RegularExpression._Lock);
                }

                return RegularExpression._Current;
            }
        }

        public Regex SingleCapturePattern { get; private set; }
        public Regex ContentOpeningPattern { get; private set; }
        public Regex ContentSeparatorPattern { get; private set; }
        public Regex ContentClosingPattern { get; private set; }

        private string CorrectForRegex(string input) =>
                input
                .Replace(".", "\\.");

        public Regex SpecificContentOpeningPattern(string directiveID, string directiveType) =>
            new Regex(string.Format(this.SpecificContentOpeningRegEx, this.CorrectForRegex(directiveID), directiveType),RegexOptions.Singleline);
    }
}
