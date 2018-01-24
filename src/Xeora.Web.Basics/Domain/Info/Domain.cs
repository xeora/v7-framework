namespace Xeora.Web.Basics.Domain.Info
{
    public class Domain
    {
        public Domain(DeploymentTypes deploymentType, string ID, Language[] languages)
        {
            this.DeploymentType = deploymentType;
            this.ID = ID;
            this.Languages = languages;
            this.Children = new DomainCollection();
        }

        /// <summary>
        /// Gets the type of the domain deployment
        /// </summary>
        /// <value>The type of the domain deployment</value>
        public DeploymentTypes DeploymentType { get; private set; }

        /// <summary>
        /// Gets the domain identifier
        /// </summary>
        /// <value>The domain identifier</value>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the available languages for the domain
        /// </summary>
        /// <value>The available languages</value>
        public Language[] Languages { get; private set; }

        /// <summary>
        /// Gets the children domains
        /// </summary>
        /// <value>The children domain collection</value>
        public DomainCollection Children { get; private set; }
    }
}
