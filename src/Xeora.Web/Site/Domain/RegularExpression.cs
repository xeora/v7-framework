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
            string directiveIdRegEx = "[\\.\\-" + characterGroup + "]+";
            string directiveIdWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            string directivePointerRegEx = "(A)?[A-Z]";
            string levelingRegEx = "\\#\\d+(\\+)?";
            string parentingRegEx = "\\[" + directiveIdRegEx + "\\]";
            string parametersRegEx = "\\(((\\|)?(" + variableRegEx + ")?)+\\)";
            string procedureRegEx = directiveIdRegEx + "\\?" + directiveIdRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

            string singleRegEx =
                "\\$" +
                "(" +
                    "(" + variableRegEx + ")\\$" + // Capture Variable
                    "|" +
                    directivePointerRegEx + "(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:" + // Capture Directive with Leveling and Parenting
                    "(" +
                        directiveIdWithSlashRegEx + "\\$" + // Capture Control without content
                        "|" +
                        procedureRegEx + "\\$" + // Capture Procedure
                    ")" + 
                ")";
            string contentOpeningRegEx = "\\$((?<DirectiveId>" + directiveIdRegEx + ")|(?<DirectiveType>" + directivePointerRegEx + ")(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:(?<DirectiveId>" + directiveIdRegEx + ")(" + parametersRegEx + ")?)\\:\\{";
            string contentSeparatorRegEx = "\\}:(?<DirectiveId>" + directiveIdRegEx + ")\\:\\{";
            string contentClosingRegEx = "\\}:(?<DirectiveId>" + directiveIdRegEx + ")\\$";

            this.SpecificContentOpeningRegEx = "\\$(({0})|({1})(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:({0}))(\\(|\\:)";
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

        private string CorrectDirectiveIdForRegex(string input) =>
            input
                .Replace(".", "\\.");

        private string CorrectDirectiveTypeForRegex(string input)
        {
            switch (input)
            {
                case "C":
                    return input.Replace("C", "(A)?C");
                case "AC":
                    return input.Replace("AC", "(A)?C");
                default:
                    return input;
            }
        }

        public Regex SpecificContentOpeningPattern(string directiveId, string directiveType) =>
            new Regex(string.Format(this.SpecificContentOpeningRegEx, this.CorrectDirectiveIdForRegex(directiveId), this.CorrectDirectiveTypeForRegex(directiveType)),RegexOptions.Singleline);
    }
}
