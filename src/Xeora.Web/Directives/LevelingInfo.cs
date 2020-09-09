using System.Text.RegularExpressions;

namespace Xeora.Web.Directives
{
    public class LevelingInfo
    {
        private LevelingInfo(int level, bool executionOnly)
        {
            this.Level = level;
            this.ExecutionOnly = executionOnly;
        }

        public int Level { get; }
        public bool ExecutionOnly { get; }

        private static readonly Regex LevelingRegEx = 
            new Regex("\\#\\d+(\\+)?", RegexOptions.Compiled);
        public static LevelingInfo Create(string value)
        {
            string[] controlValueParts = value.Split(':');

            Match cLevelingMatch = 
                LevelingInfo.LevelingRegEx.Match(controlValueParts[0]);

            if (!cLevelingMatch.Success) return new LevelingInfo(0, true);
            
            // Trim # character from match result
            string cleanValue = 
                cLevelingMatch.Value.Substring(1, cLevelingMatch.Length - 1);
            bool executionOnly = true;

            if (cleanValue.IndexOf('+') > -1)
            {
                executionOnly = false;
                cleanValue = cleanValue.Substring(0, cleanValue.IndexOf('+'));
            }
            int.TryParse(cleanValue, out int level);

            return new LevelingInfo(level, executionOnly);
        }
    }
}