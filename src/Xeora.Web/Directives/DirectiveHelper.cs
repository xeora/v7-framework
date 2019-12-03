using System.Text.RegularExpressions;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public static class DirectiveHelper
    {
        private static readonly Regex BoundDirectiveIdRegEx =
            new Regex("\\[[\\.\\w\\-]+\\]", RegexOptions.Compiled);
        public static string CaptureBoundDirectiveId(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper.BoundDirectiveIdRegEx.Match(controlValueSplitted[0]);

            return cpIdMatch.Success ? cpIdMatch.Value.Substring(1, cpIdMatch.Length - 2) : string.Empty;
        }

        private static readonly Regex DirectiveIdRegEx =
            new Regex("[\\/\\.\\w\\-]+", RegexOptions.Compiled);
        public static string CaptureDirectiveId(string rawValue)
        {
            string[] controlValueSplitted = rawValue.Split(':');

            Match cpIdMatch =
                DirectiveHelper.DirectiveIdRegEx.Match(controlValueSplitted[1]);

            return cpIdMatch.Success ? cpIdMatch.Value : string.Empty;
        }

        private static readonly Regex ControlParametersRegEx =
            new Regex("\\(((\\|)?(([#]+|[\\^\\-\\+\\*\\~\\&])?([0-9_a-zA-Z]+)|\\=[\\S]*|@([#]+|[\\-])?([0-9_a-zA-Z]+\\.)[\\.0-9_a-zA-Z]+)?)+\\)", RegexOptions.Compiled);
        public static string[] CaptureControlParameters(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper.ControlParametersRegEx.Match(controlValueSplitted[1]);

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
            switch (directiveType)
            {
                case "C":
                    return DirectiveTypes.Control;
                case "AC":
                    return DirectiveTypes.ControlAsync;
                case "T":
                    return DirectiveTypes.Template;
                case "L":
                    return DirectiveTypes.Translation;
                case "O":
                    return DirectiveTypes.ReplaceableTranslation;
                case "P":
                    return DirectiveTypes.HashCodePointedTemplate;
                case "F":
                    return DirectiveTypes.Execution;
                case "S":
                    return DirectiveTypes.InLineStatement;
                case "H":
                    return DirectiveTypes.UpdateBlock;
                case "E":
                    return DirectiveTypes.PermissionBlock;
                case "XF":
                    return DirectiveTypes.EncodedExecution;
                case "MB":
                    return DirectiveTypes.MessageBlock;
                case "PC":
                    return DirectiveTypes.PartialCache;
                case "AG":
                    return DirectiveTypes.AsyncGroup;
                default:
                    return DirectiveTypes.Undefined;
            }
        }
    }
}