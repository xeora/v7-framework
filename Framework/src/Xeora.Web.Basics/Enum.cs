namespace Xeora.Web.Basics
{
    public class Enum
    {
        public enum PageCachingTypes
        {
            AllContent,
            AllContentCookiless,
            TextsOnly,
            TextsOnlyCookiless,
            NoCache,
            NoCacheCookiless
        }

        public enum RequestTagFilteringTypes
        {
            None,
            OnlyForm,
            OnlyQuery,
            Both
        }
    }
}
