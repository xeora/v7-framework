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
            const string simpleVariableRegEx = "([#]+|[\\^\\-\\+\\*\\~\\&])?([" + characterGroup + "]+)";
            const string staticVariableRegEx = "\\=[\\S]*";
            const string objectVariableRegEx = "@([#]+|[\\-\\&\\.])?([" + characterGroup + "]+\\.)[\\." + characterGroup + "]+";
            const string variableRegEx = simpleVariableRegEx + "|" + staticVariableRegEx + "|" + objectVariableRegEx;
            const string directiveIdRegEx = "[\\.\\-" + characterGroup + "]+";
            const string directiveIdWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            const string directivePointerRegEx = "(A)?[A-Z]";
            const string levelingRegEx = "\\#\\d+(\\+)?";
            const string parentingRegEx = "\\[" + directiveIdRegEx + "\\]";
            const string parametersRegEx = "\\(((\\|)?(" + variableRegEx + ")?)+\\)";
            const string procedureRegEx = directiveIdRegEx + "\\?" + directiveIdRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

            const string singleRegEx =
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
            const string contentOpeningRegEx = "\\$((?<DirectiveId>" + directiveIdRegEx + ")|(?<DirectiveType>" + directivePointerRegEx + ")(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:(?<DirectiveId>" + directiveIdRegEx + ")(" + parametersRegEx + ")?)\\:\\{";
            const string contentSeparatorRegEx = "\\}:(?<DirectiveId>" + directiveIdRegEx + ")\\:\\{";
            const string contentClosingRegEx = "\\}:(?<DirectiveId>" + directiveIdRegEx + ")\\$";

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
        private static RegularExpression _current;
        public static RegularExpression Current
        {
            get
            {
                Monitor.Enter(RegularExpression.Lock);
                try
                {
                    return RegularExpression._current ?? (RegularExpression._current = new RegularExpression());
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

        private static string CorrectDirectiveIdForRegex(string input) =>
            input.Replace(".", "\\.");

        private static string CorrectDirectiveTypeForRegex(string input)
        {
            return input switch
            {
                "C" => input.Replace("C", "(A)?C"),
                "AC" => input.Replace("AC", "(A)?C"),
                _ => input
            };
        }

        public Regex SpecificContentOpeningPattern(string directiveId, string directiveType) =>
            new Regex(string.Format(this._SpecificContentOpeningRegEx, RegularExpression.CorrectDirectiveIdForRegex(directiveId), RegularExpression.CorrectDirectiveTypeForRegex(directiveType)),RegexOptions.Singleline);
    }
}
