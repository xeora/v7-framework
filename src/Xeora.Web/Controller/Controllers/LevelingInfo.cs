using System.Text.RegularExpressions;

namespace Xeora.Web.Controller.Directive
{
    public class LevelingInfo
    {
        private LevelingInfo(int level, bool executionOnly)
        {
            this.Level = level;
            this.ExecutionOnly = executionOnly;
        }

        public int Level { get; private set; }
        public bool ExecutionOnly { get; private set; }

        private static Regex _LevelingRegEx = 
            new Regex("\\#\\d+(\\+)?", RegexOptions.Compiled);
        public static LevelingInfo Create(string value)
        {
            int level = 0;
            bool executionOnly = true;

            string[] controlValueSplitted = value.Split(':');

            Match cLevelingMatch = LevelingInfo._LevelingRegEx.Match(controlValueSplitted[0]);

            if (cLevelingMatch.Success)
            {
                // Trim # character from match result
                string cleanValue = cLevelingMatch.Value.Substring(1, cLevelingMatch.Value.Length - 1);

                if (cleanValue.IndexOf('+') > -1)
                {
                    executionOnly = false;
                    cleanValue = cleanValue.Substring(0, cleanValue.IndexOf('+'));
                }

                int.TryParse(cleanValue, out level);
            }

            return new LevelingInfo(level, executionOnly);
        }
    }
}