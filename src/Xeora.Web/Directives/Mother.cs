using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;
using Xeora.Web.Site.Setting.Control;

namespace Xeora.Web.Directives
{
    public class Mother : IMother
    {
        private readonly DirectivePool _Pool;

        public event ParsingHandler ParseRequested;
        public event InstanceHandler InstanceRequested;
        public event DeploymentAccessHandler DeploymentAccessRequested;
        public event ControlResolveHandler ControlResolveRequested;

        public Mother(Basics.ControlResult.Message messageResult, string[] updateBlockIDStack)
        {
            this._Pool = new DirectivePool();
            this.Scheduler = new DirectiveScheduler(ref this._Pool);
            this.UpdateBlockIDStack = new Stack<string>();

            this.MessageResult = messageResult;
            if (updateBlockIDStack != null && updateBlockIDStack.Length > 0)
                foreach(string updateBlockControlID in updateBlockIDStack)
                    this.UpdateBlockIDStack.Push(updateBlockControlID);

            this.Directives = new DirectiveCollection(this, null);
        }

        public DirectivePool Pool => this._Pool;
        public DirectiveScheduler Scheduler { get; private set; }
        public Basics.ControlResult.Message MessageResult { get; private set; }
        public Stack<string> UpdateBlockIDStack { get; private set; }
        public DirectiveCollection Directives { get; private set; }

        public void RequestParsing(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments) =>
            ParseRequested?.Invoke(rawValue, ref childrenContainer, arguments);

        public void RequestInstance(ref IDomain instance) =>
            InstanceRequested?.Invoke(ref instance);

        public void RequestDeploymentAccess(ref IDomain instance, ref Deployment.Domain deployment) =>
            DeploymentAccessRequested?.Invoke(ref instance, ref deployment);

        public void RequestControlResolve(string controlID, ref IDomain instance, out IBase control)
        {
            control = new Unknown();
            ControlResolveRequested?.Invoke(controlID, ref instance, out control);
        }
    }
}