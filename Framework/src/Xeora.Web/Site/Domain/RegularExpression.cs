using System.Text.RegularExpressions;

namespace Xeora.Web.Site
{
    public class RegularExpression
    {
        private RegularExpression()
        {
            // CAPTURE REGULAR EXPRESSIONS
            //\$  ( ( ([#]+|[\^\-\+\*\~])?(\w+) | @[#\-]*(\w+\.)[\w+\.]+ ) \$ | [\.\w\-]+\:\{ | \w(\#\d+(\+)?)?(\[[\.\w\-]+\])?\: ( [\/\.\w\-]+\$ | [\.\w\-]+\:\{ | [\.\w\-]+\?[\.\w\-]+   (\,   (   (\|)?    ( [#\.\^\-\+\*\~]*\w+  |  \=[\S]+  |  @[#\-]*(\w+\.)[\w+\.]+ )?   )*  )?  \$ )) | \}\:[\.\w\-]+\:\{ | \}\:[\.\w\-]+ \$           [\w\.\,\-\+]
            string captureRegEx = "\\$((([#]+|[\\^\\-\\+\\*\\~])?(\\w+)|@[#\\-]*(\\w+\\.)[\\w+\\.]+)\\$|[\\.\\w\\-]+\\:\\{|\\w(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?\\:([\\/\\.\\w\\-]+\\$|[\\.\\w\\-]+\\:\\{|[\\.\\w\\-]+\\?[\\.\\w\\-]+(\\,((\\|)?([#\\.\\^\\-\\+\\*\\~]*([\\w+][^\\$]*)|\\=([\\S+][^\\$]*)|@[#\\-]*(\\w+\\.)[\\w+\\.]+)?)*)?\\$))|\\}\\:[\\.\\w\\-]+\\:\\{|\\}\\:[\\.\\w\\-]+\\$";
            string bracketedRegExOpening = "\\$((?<ItemID>\\w+)|(?<DirectiveType>\\w)(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?\\:(?<ItemID>[\\.\\w\\-]+))\\:\\{";
            string bracketedRegExSeparator = "\\}:(?<ItemID>[\\.\\w\\-]+)\\:\\{";
            string bracketedRegExClosing = "\\}:(?<ItemID>[\\.\\w\\-]+)\\$";
            // !---

            this.MainCapturePattern =
                new Regex(captureRegEx, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerOpenPattern =
                new Regex(bracketedRegExOpening, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerSeparatorPattern =
                new Regex(bracketedRegExSeparator, RegexOptions.Multiline | RegexOptions.Compiled);
            this.BracketedControllerClosePattern =
                new Regex(bracketedRegExClosing, RegexOptions.Multiline | RegexOptions.Compiled);

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
    }
}
