using System.Text.RegularExpressions;

namespace Xeora.Web.Directives
{
    public static class DirectiveHelper
    {
        private static readonly Regex BoundDirectiveIdRegEx =
            new Regex("\\[[\\.\\w\\-]+\\]", RegexOptions.Compiled);
        public static string CaptureBoundDirectiveId(string value)
        {
            string[] controlValueParts = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper.BoundDirectiveIdRegEx.Match(controlValueParts[0]);

            return cpIdMatch.Success ? cpIdMatch.Value.Substring(1, cpIdMatch.Length - 2) : string.Empty;
        }

        private static readonly Regex DirectiveIdRegEx =
            new Regex("[\\/\\.\\w\\-]+", RegexOptions.Compiled);
        public static string CaptureDirectiveId(string rawValue)
        {
            string[] controlValueParts = rawValue.Split(':');

            Match cpIdMatch =
                DirectiveHelper.DirectiveIdRegEx.Match(controlValueParts[1]);

            return cpIdMatch.Success ? cpIdMatch.Value : string.Empty;
        }

        private static readonly Regex DirectiveParametersRegEx =
            new Regex("\\(((\\|)?(([#]+|[\\^\\-\\+\\*\\~\\&])?([0-9_a-zA-Z]+)|\\=[\\S]*|@([#]+|[\\-\\&\\.])?([0-9_a-zA-Z]+\\.)[\\.0-9_a-zA-Z]+)?)+\\)", RegexOptions.Compiled);
        public static string[] CaptureDirectiveParameters(string value, bool specialDirective)
        {
            string[] controlValueParts = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper.DirectiveParametersRegEx.Match(controlValueParts[specialDirective ? 0 : 1]);

            if (!cpIdMatch.Success) return new string[] { };
            
            // Trim ( and ) character from match result
            string rawParameters =
                cpIdMatch.Value.Substring(1, cpIdMatch.Length - 2);

            return rawParameters.Split('|');
        }

        private static readonly Regex ParameterPointerRegEx =
            new Regex("^\\{(?<index>\\d+)\\}$", RegexOptions.Compiled);
        public static int CaptureParameterPointer(string value)
        {
            Match match =
                DirectiveHelper.ParameterPointerRegEx.Match(value);

            return match.Success ? int.Parse(match.Groups["index"].Value) : -1;
        }

        private static readonly Regex DirectiveTypeRegEx =
            new Regex("\\$(((?<DirectiveType>(A)?\\w)(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?)|(?<DirectiveType>\\w+))\\:", RegexOptions.Compiled);
        public static DirectiveTypes CaptureDirectiveType(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return DirectiveTypes.Undefined;

            Match cpIdMatch = DirectiveHelper.DirectiveTypeRegEx.Match(rawValue);

            if (!cpIdMatch.Success) return DirectiveTypes.Property;
            
            string directiveType = 
                cpIdMatch.Result("${DirectiveType}");
            return directiveType switch
            {
                "C" => DirectiveTypes.Control,
                "AC" => DirectiveTypes.ControlAsync,
                "T" => DirectiveTypes.Template,
                "L" => DirectiveTypes.Translation,
                "O" => DirectiveTypes.ReplaceableTranslation,
                "P" => DirectiveTypes.HashCodePointedTemplate,
                "F" => DirectiveTypes.Execution,
                "S" => DirectiveTypes.InLineStatement,
                "H" => DirectiveTypes.UpdateBlock,
                "E" => DirectiveTypes.PermissionBlock,
                "XF" => DirectiveTypes.EncodedExecution,
                "MB" => DirectiveTypes.MessageBlock,
                "PC" => DirectiveTypes.PartialCache,
                "AG" => DirectiveTypes.AsyncGroup,
                _ => DirectiveTypes.Undefined
            };
        }
    }
}