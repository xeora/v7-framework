namespace Xeora.Web.Basics.Domain.Info
{
    public class Language
    {
        public Language(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the language identifier
        /// </summary>
        /// <value>The language identifier</value>
        public string Id { get; }

        /// <summary>
        /// Gets the language human readable name
        /// </summary>
        /// <value>The language human readable name</value>
        public string Name { get; }
    }
}
