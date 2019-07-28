using System;

namespace Xeora.Web.Directives.Controls
{
    public class ControlHelper
    {
        public static string CleanJavascriptSignature(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            const string javascriptSignature = "javascript:";

            if (input.IndexOf(javascriptSignature, StringComparison.Ordinal) == 0)
                input = input.Substring(javascriptSignature.Length);

            if (input[input.Length - 1] == ';')
                input = input.Substring(0, input.Length - 1);

            return input;
        }
    }
}