using System.Text.RegularExpressions;

namespace Xeora.Web.Controller.Directive
{
    public class DirectiveHelper
    {
        private static Regex _BoundControlIDRegEx = 
            new Regex("\\[[\\.\\w\\-]+\\]", RegexOptions.Compiled);
        public static string CaptureBoundControlID(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIDMatch = DirectiveHelper._BoundControlIDRegEx.Match(controlValueSplitted[0]);

            if (cpIDMatch.Success)
            {
                // Trim [ and ] characters from match result
                return cpIDMatch.Value.Substring(1, cpIDMatch.Value.Length - 2);
            }

            return string.Empty;
        }

        private static Regex _ControlIDRegEx = 
            new Regex("[\\/\\.\\w\\-]+", RegexOptions.Compiled);
        public static string CaptureControlID(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIDMatch = DirectiveHelper._ControlIDRegEx.Match(controlValueSplitted[1]);

            if (cpIDMatch.Success)
                return cpIDMatch.Value;

            return string.Empty;
        }

        private static Regex _ControlParametersRegEx =
            new Regex("\\(((\\|)?(([#]+|[\\^\\-\\+\\*\\~])?([0-9_a-zA-Z]+)|\\=([\\S+][^\\$\\|]*)|@([#]+|[\\-])?([0-9_a-zA-Z]+\\.)[\\.0-9_a-zA-Z]+)?)+\\)", RegexOptions.Compiled);
        public static string[] CaptureControlParameters(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIDMatch = DirectiveHelper._ControlParametersRegEx.Match(controlValueSplitted[1]);

            if (cpIDMatch.Success)
            {
                // Trim ( and ) character from match result
                string rawParameters = 
                    cpIDMatch.Value.Substring(1, cpIDMatch.Value.Length - 2);

                return rawParameters.Split('|');
            }

            return new string[] { };
        }

        private static Regex _DirectiveTypeRegEx = 
            new Regex("\\$(((?<DirectiveType>\\w)(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?)|(?<DirectiveType>\\w+))\\:", RegexOptions.Compiled);
        public static DirectiveTypes CaptureDirectiveType(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return DirectiveTypes.Undefined;

            Match cpIDMatch = DirectiveHelper._DirectiveTypeRegEx.Match(rawValue);

            if (cpIDMatch.Success)
            {
                string DirectiveType = cpIDMatch.Result("${DirectiveType}");

                switch (DirectiveType)
                {
                    case "C":
                        return DirectiveTypes.Control;
                    case "T":
                        return DirectiveTypes.Template;
                    case "L":
                        return DirectiveTypes.Translation;
                    case "O":
                        return DirectiveTypes.FormattableTranslation;
                    case "P":
                        return DirectiveTypes.HashCodePointedTemplate;
                    case "F":
                        return DirectiveTypes.Execution;
                    case "S":
                        return DirectiveTypes.InLineStatement;
                    case "H":
                        return DirectiveTypes.UpdateBlock;
                    case "XF":
                        return DirectiveTypes.EncodedExecution;
                    case "MB":
                        return DirectiveTypes.MessageBlock;
                    case "PC":
                        return DirectiveTypes.PartialCache;
                    default:
                        // Capital Test
                        string lowerDirectiveType = DirectiveType.ToLower();

                        if (string.Compare(DirectiveType, lowerDirectiveType) == 0)
                            throw new Exception.DirectivePointerException();

                        break;
                }
            }

            return DirectiveTypes.Undefined;
        }
    }
}