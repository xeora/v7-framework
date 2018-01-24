namespace Xeora.Web.Basics.Domain.Info
{
    public class Language
    {
        public Language(string ID, string name)
        {
            this.ID = ID;
            this.Name = name;
        }

        /// <summary>
        /// Gets the language identifier
        /// </summary>
        /// <value>The language identifier</value>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the language human readable name
        /// </summary>
        /// <value>The language human readable name</value>
        public string Name { get; private set; }
    }
}
