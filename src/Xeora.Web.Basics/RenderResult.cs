namespace Xeora.Web.Basics
{
    public class RenderResult
    {
        public RenderResult(string content, bool hasErrors)
        {
            this.Content = content;
            this.HasErrors = hasErrors;
        }

        /// <summary>
        /// Gets the content of render
        /// </summary>
        /// <value>Render Content</value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets a value indicating whether render content has inline errors
        /// </summary>
        /// <value><c>true</c> if has error(s); otherwise, <c>false</c></value>
        public bool HasErrors { get; private set; }
    }
}
