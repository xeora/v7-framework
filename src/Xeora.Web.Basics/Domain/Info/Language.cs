namespace Xeora.Web.Basics.Domain.Info
{
    public class Language
    {
        public Language(string Id, string name)
        {
            this.Id = Id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the language identifier
        /// </summary>
        /// <value>The language identifier</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the language human readable name
        /// </summary>
        /// <value>The language human readable name</value>
        public string Name { get; private set; }
    }
}
