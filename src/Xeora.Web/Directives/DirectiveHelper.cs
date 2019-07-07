using System.Text.RegularExpressions;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public static class DirectiveHelper
    {
        private static readonly Regex _BoundDirectiveIdRegEx =
            new Regex("\\[[\\.\\w\\-]+\\]", RegexOptions.Compiled);
        public static string CaptureBoundDirectiveId(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper._BoundDirectiveIdRegEx.Match(controlValueSplitted[0]);

            if (cpIdMatch.Success)
            {
                // Trim [ and ] characters from match result
                return cpIdMatch.Value.Substring(1, cpIdMatch.Length - 2);
            }

            return string.Empty;
        }

        private static readonly Regex _DirectiveIdRegEx =
            new Regex("[\\/\\.\\w\\-]+", RegexOptions.Compiled);
        public static string CaptureDirectiveId(string rawValue)
        {
            string[] controlValueSplitted = rawValue.Split(':');

            Match cpIdMatch =
                DirectiveHelper._DirectiveIdRegEx.Match(controlValueSplitted[1]);

            if (cpIdMatch.Success)
                return cpIdMatch.Value;

            return string.Empty;
        }

        private static readonly Regex _ControlParametersRegEx =
            new Regex("\\(((\\|)?(([#]+|[\\^\\-\\+\\*\\~])?([0-9_a-zA-Z]+)|\\=[\\S]*|@([#]+|[\\-])?([0-9_a-zA-Z]+\\.)[\\.0-9_a-zA-Z]+)?)+\\)", RegexOptions.Compiled);
        public static string[] CaptureControlParameters(string value)
        {
            string[] controlValueSplitted = value.Split(':');

            Match cpIdMatch =
                DirectiveHelper._ControlParametersRegEx.Match(controlValueSplitted[1]);

            if (cpIdMatch.Success)
            {
                // Trim ( and ) character from match result
                string rawParameters =
                    cpIdMatch.Value.Substring(1, cpIdMatch.Length - 2);

                return rawParameters.Split('|');
            }

            return new string[] { };
        }

        private static readonly Regex _ParameterPointerRegEx =
            new Regex("^\\{(?<index>\\d+)\\}$", RegexOptions.Compiled);
        public static int CaptureParameterPointer(string value)
        {
            Match match =
                DirectiveHelper._ParameterPointerRegEx.Match(value);

            if (!match.Success)
                return -1;

            return int.Parse(match.Groups["index"].Value);
        }

        private static readonly Regex _DirectiveTypeRegEx =
            new Regex("\\$(((?<DirectiveType>(A)?\\w)(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?)|(?<DirectiveType>\\w+))\\:", RegexOptions.Compiled);
        public static DirectiveTypes CaptureDirectiveType(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return DirectiveTypes.Undefined;

            Match cpIdMatch = DirectiveHelper._DirectiveTypeRegEx.Match(rawValue);

            if (cpIdMatch.Success)
            {
                string DirectiveType = cpIdMatch.Result("${DirectiveType}");

                switch (DirectiveType)
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
                        return DirectiveTypes.FormattableTranslation;
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
                }
            }

            return DirectiveTypes.Property;
        }

        public static object RenderProperty(IDirective parent, string rawValue, Global.ArgumentCollection arguments, string requesterUniqueId)
        {
            DirectiveCollection directives = 
                new DirectiveCollection(parent.Mother, parent);

            Property property = new Property(rawValue, arguments);

            directives.Add(property);
            directives.Render(requesterUniqueId);

            parent.Deliver(RenderStatus.Rendering, string.Empty);

            return property.ObjectResult;
        }
    }
}