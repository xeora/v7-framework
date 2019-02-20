using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Directives;
using Xeora.Web.Global;

namespace Xeora.Web.Site
{
    internal class RenderEngine : Basics.IRenderEngine
    {
        private readonly Basics.Domain.IDomain _Domain;

        public RenderEngine(Basics.Domain.IDomain domain) =>
            this._Domain = domain ?? throw new System.Exception("Domain instance required!");

        public Basics.RenderResult Render(Basics.ServiceDefinition serviceDefinition, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null) =>
            this.Render(string.Format("$T:{0}$", serviceDefinition.FullPath), messageResult, updateBlockControlIDStack);

        public Basics.RenderResult Render(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null)
        {
            Mother mother =
                new Mother(messageResult, updateBlockControlIDStack);
            mother.ParseRequested += this.OnParseRequest;
            mother.InstanceRequested += this.OnInstanceRequest;
            mother.DeploymentAccessRequested += this.OnDeploymentAccessRequest;
            mother.ControlResolveRequested += this.OnControlResolveRequest;

            Directives.Single workingDirective = 
                new Directives.Single(xeoraContent, null);

            mother.Directives.Add(workingDirective);
            mother.Directives.Render(null);

            return new Basics.RenderResult(workingDirective.Result, workingDirective.HasInlineError);
        }

        private void OnParseRequest(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments)
        {
            DateTime begins = DateTime.Now;

            List<IDirective> directives = 
                new List<IDirective>();

            Parser.Parse(directives.Add, rawValue, arguments);

            childrenContainer.AddRange(directives);

            TimeSpan duration = DateTime.Now.Subtract(begins);
            Basics.Console.Push(rawValue, duration.TotalMilliseconds.ToString(), duration.TotalMilliseconds > 100 ? rawValue : string.Empty, false, true);
        }

        private void OnDeploymentAccessRequest(ref Basics.Domain.IDomain domain, ref Deployment.Domain deployment) =>
            deployment = ((Domain)domain).Deployment;

        private void OnInstanceRequest(ref Basics.Domain.IDomain domain) =>
            domain = this._Domain;

        private static ConcurrentDictionary<string, IBase> _ControlsCache =
            new ConcurrentDictionary<string, IBase>();
        private void OnControlResolveRequest(string controlID, ref Basics.Domain.IDomain domain, out IBase control)
        {
            do
            {
                control = 
                    ((Domain)domain).Controls.Select(controlID);

                if (control.Type != Basics.Domain.Control.ControlTypes.Unknown)
                    return;

                domain = domain.Parent;
            } while (domain != null);
        }

        public void ClearCache() =>
            RenderEngine._ControlsCache.Clear();
    }
}