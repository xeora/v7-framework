using System.Text.RegularExpressions;
using System.Threading;

namespace Xeora.Web.Application
{
    public class RegularExpression
    {
        private readonly string _SpecificContentOpeningRegEx;

        private RegularExpression()
        {
            // CAPTURE REGULAR EXPRESSIONS
            const string characterGroup = "0-9_a-zA-Z";
            const string simpleVariableRegEx = "([#]+|[\\^\\-\\+\\*\\~])?([" + characterGroup + "]+)";
            const string staticVariableRegEx = "\\=[\\S]*";
            const string objectVariableRegEx = "@([#]+|[\\-])?([" + characterGroup + "]+\\.)[\\." + characterGroup + "]+";
            const string variableRegEx = simpleVariableRegEx + "|" + staticVariableRegEx + "|" + objectVariableRegEx;
            const string directiveIdRegEx = "[\\.\\-" + characterGroup + "]+";
            const string directiveIdWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            const string directivePointerRegEx = "(A)?[A-Z]";
            const string levelingRegEx = "\\#\\d+(\\+)?";
            const string parentingRegEx = "\\[" + directiveIdRegEx + "\\]";
            const string parametersRegEx = "\\(((\\|)?(" + variableRegEx + ")?)+\\)";
            const string procedureRegEx = directiveIdRegEx + "\\?" + directiveIdRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

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

            this._SpecificContentOpeningRegEx = "\\$(({0})|({1})(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:({0}))(\\(|\\:)";
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

        private static readonly object Lock = new object();
        private static RegularExpression _Current;
        public static RegularExpression Current
        {
            get
            {
                Monitor.Enter(RegularExpression.Lock);
                try
                {
                    return RegularExpression._Current ?? (RegularExpression._Current = new RegularExpression());
                }
                finally
                {
                    Monitor.Exit(RegularExpression.Lock);
                }
            }
        }

        public Regex SingleCapturePattern { get; }
        public Regex ContentOpeningPattern { get; }
        public Regex ContentSeparatorPattern { get; }
        public Regex ContentClosingPattern { get; }

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
            new Regex(string.Format(this._SpecificContentOpeningRegEx, this.CorrectDirectiveIdForRegex(directiveId), this.CorrectDirectiveTypeForRegex(directiveType)),RegexOptions.Singleline);
    }
}
