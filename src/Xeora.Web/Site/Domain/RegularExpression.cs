using System.Text.RegularExpressions;
using System.Threading;

namespace Xeora.Web.Site
{
    public class RegularExpression
    {
        private RegularExpression()
        {
            // CAPTURE REGULAR EXPRESSIONS
            string characterGroup = "0-9_a-zA-Z";
            string simpleVariableRegEx = "([#]+|[\\^\\-\\+\\*\\~])?([" + characterGroup + "]+)";
            string staticVariableRegEx = "\\=[\\S]*";
            string objectVariableRegEx = "@([#]+|[\\-])?([" + characterGroup + "]+\\.)[\\." + characterGroup + "]+";
            string variableRegEx = simpleVariableRegEx + "|" + staticVariableRegEx + "|" + objectVariableRegEx;
            string tagIDRegEx = "[\\.\\-" + characterGroup + "]+";
            string tagIDWithSlashRegEx = "[\\/\\.\\-" + characterGroup + "]+"; // for template capturing
            string directivePointerRegEx = "[A-Z]";
            string levelingRegEx = "\\#\\d+(\\+)?";
            string parentingRegEx = "\\[" + tagIDRegEx + "\\]";
            string parametersRegEx = "\\(((\\|)?(" + variableRegEx + ")?)+\\)";
            string procedureRegEx = tagIDRegEx + "\\?" + tagIDRegEx + "(\\,((\\|)?(" + variableRegEx + ")?)*)?";

            /*
             * Directive Index Marker is implemented for the workaround purpose of .net regex insufficiency
             * When the raw input with marked directive is pushed for parsing to regex, it parses correctly
             * but inefficiently. It takes around 4 seconds to parse for a simple input. If you track the
             * index marker on directives, it takes 0 ms. So Track the index marker and skip in the loop
             * in Domain.cs file line ~183.
             */
            string directiveIndexMarkerRegEx = "\\~\\d+";

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
                        tagIDRegEx + "(" + parametersRegEx + ")?(" + directiveIndexMarkerRegEx + ")?\\:\\{" + // Capture Control with content and Parameters(opening)
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
            string bracketedRegExOpening = "\\$((?<ItemID>" + tagIDRegEx + ")|(?<DirectiveType>" + directivePointerRegEx + ")(" + levelingRegEx + ")?(" + parentingRegEx + ")?\\:(?<ItemID>" + tagIDRegEx + ")(" + parametersRegEx + ")?(?<DirectiveIndex>" + directiveIndexMarkerRegEx + ")?)\\:\\{";
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

        public Regex MainCapturePattern { get; private set; }
        public Regex BracketedControllerOpenPattern { get; private set; }
        public Regex BracketedControllerSeparatorPattern { get; private set; }
        public Regex BracketedControllerClosePattern { get; private set; }
        public Regex VariableCapturePattern { get; private set; }
    }
}
