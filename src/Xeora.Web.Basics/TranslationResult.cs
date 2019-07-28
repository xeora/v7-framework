namespace Xeora.Web.Basics
{
    public class TranslationResult
    {
        public TranslationResult(bool translated, string translation)
        {
            this.Translated = translated;
            this.Translation = translation;
        }

        /// <summary>
        /// Gets a value indicating whether request is translated
        /// </summary>
        /// <value><c>true</c> if is translated; otherwise, <c>false</c></value>
        public bool Translated { get; }

        /// <summary>
        /// Gets the Translation result
        /// </summary>
        /// <value>The Translation Result</value>
        public string Translation { get; }
    }
}
