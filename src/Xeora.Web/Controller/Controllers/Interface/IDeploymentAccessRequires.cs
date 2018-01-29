namespace Xeora.Web.Controller.Directive
{
    public delegate void DeploymentAccessHandler(ref Basics.Domain.IDomain workingInstance, ref Deployment.Domain deployment);
    public interface IDeploymentAccessRequires
    {
        event DeploymentAccessHandler DeploymentAccessRequested;
    }
}