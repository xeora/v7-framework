using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;
using Xeora.Web.Service.Workers;

namespace Xeora.Web.Directives
{
    public delegate void ParsingHandler(string rawValue, DirectiveCollection childrenContainer, ArgumentCollection arguments);
    public delegate void InstanceHandler(out IDomain instance);
    public delegate void DeploymentAccessHandler(ref IDomain instance, out Deployment.Domain deployment);
    public delegate void ControlResolveHandler(string controlId, ref IDomain instance, out IBase control);

    public interface IMother
    {
        object PropertyLock { get; }
        Bucket Bucket { get; }

        ConcurrentDictionary<DirectiveTypes, Tuple<int, double>> AnalysisBulk { get; }
        DirectivePool Pool { get; }

        Basics.ControlResult.Message MessageResult { get; }
        List<string> RequestedUpdateBlockIds { get; }

        void RequestParsing(string rawValue, DirectiveCollection childrenContainer, ArgumentCollection arguments);
        void RequestInstance(out IDomain instance);
        void RequestDeploymentAccess(ref IDomain instance, out Deployment.Domain deployment);
        void RequestControlResolve(string controlId, ref IDomain instance, out IBase control);
    }
}
