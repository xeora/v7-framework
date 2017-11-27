namespace Xeora.Web.Controller.Directive
{
    public delegate void DeploymentAccessHandler(ref Basics.IDomain workingInstance, ref Deployment.DomainDeployment domainDeployment);
    public interface IDeploymentAccessRequires
    {
        event DeploymentAccessHandler DeploymentAccessRequested;
    }
}