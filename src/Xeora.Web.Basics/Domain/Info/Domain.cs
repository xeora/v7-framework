namespace Xeora.Web.Basics.Domain.Info
{
    public class Domain
    {
        public Domain(DeploymentTypes deploymentType, string id, Language[] languages)
        {
            this.DeploymentType = deploymentType;
            this.Id = id;
            this.Languages = languages;
            this.Children = new DomainCollection();
        }

        /// <summary>
        /// Gets the type of the domain deployment
        /// </summary>
        /// <value>The type of the domain deployment</value>
        public DeploymentTypes DeploymentType { get; }

        /// <summary>
        /// Gets the domain identifier
        /// </summary>
        /// <value>The domain identifier</value>
        public string Id { get; }

        /// <summary>
        /// Gets the available languages for the domain
        /// </summary>
        /// <value>The available languages</value>
        public Language[] Languages { get; }

        /// <summary>
        /// Gets the children domains
        /// </summary>
        /// <value>The children domain collection</value>
        public DomainCollection Children { get; }
    }
}
