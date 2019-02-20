using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;

namespace Xeora.Web.Directives
{
    public delegate void ParsingHandler(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments);
    public delegate void InstanceHandler(ref IDomain instance);
    public delegate void DeploymentAccessHandler(ref IDomain instance, ref Deployment.Domain deployment);
    public delegate void ControlResolveHandler(string controlID, ref IDomain instance, out IBase control);

    public interface IMother
    {
        DirectivePool Pool { get; }
        DirectiveScheduler Scheduler { get; }

        Basics.ControlResult.Message MessageResult { get; }
        Stack<string> UpdateBlockIDStack { get; }

        DirectiveCollection Directives { get; }

        void RequestParsing(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments);
        void RequestInstance(ref IDomain instance);
        void RequestDeploymentAccess(ref IDomain instance, ref Deployment.Domain deployment);
        void RequestControlResolve(string controlID, ref IDomain instance, out IBase control);
    }
}
