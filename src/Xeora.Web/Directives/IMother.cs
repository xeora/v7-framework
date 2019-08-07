using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;

namespace Xeora.Web.Directives
{
    public delegate void ParsingHandler(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments);
    public delegate void InstanceHandler(out IDomain instance);
    public delegate void DeploymentAccessHandler(ref IDomain instance, out Deployment.Domain deployment);
    public delegate void ControlResolveHandler(string controlId, ref IDomain instance, out IBase control);

    public interface IMother
    {
        DirectivePool Pool { get; }

        Basics.ControlResult.Message MessageResult { get; }
        List<string> RequestedUpdateBlockIds { get; }

        void RequestParsing(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments);
        void RequestInstance(out IDomain instance);
        void RequestDeploymentAccess(ref IDomain instance, out Deployment.Domain deployment);
        void RequestControlResolve(string controlId, ref IDomain instance, out IBase control);
    }
}
