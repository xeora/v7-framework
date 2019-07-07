using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;
using Xeora.Web.Site.Setting.Control;

namespace Xeora.Web.Directives
{
    public class Mother : IMother
    {
        private readonly IDirective _Directive;
        private readonly DirectivePool _Pool;

        public event ParsingHandler ParseRequested;
        public event InstanceHandler InstanceRequested;
        public event DeploymentAccessHandler DeploymentAccessRequested;
        public event ControlResolveHandler ControlResolveRequested;

        public Mother(IDirective directive, Basics.ControlResult.Message messageResult, string[] updateBlockIdStack)
        {
            this._Pool = new DirectivePool();
            this.UpdateBlockIdStack = new Stack<string>();

            this.MessageResult = messageResult;
            if (updateBlockIdStack != null && updateBlockIdStack.Length > 0)
                foreach (string updateBlockControlId in updateBlockIdStack)
                    this.UpdateBlockIdStack.Push(updateBlockControlId);

            this._Directive = directive;
            this._Directive.Mother = this;
        }

        public Mother(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockIdStack) :
            this(new Single(xeoraContent, null), messageResult, updateBlockIdStack)
        { }

        public DirectivePool Pool => this._Pool;
        public Basics.ControlResult.Message MessageResult { get; private set; }
        public Stack<string> UpdateBlockIdStack { get; private set; }

        public void RequestParsing(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments) =>
            ParseRequested?.Invoke(rawValue, ref childrenContainer, arguments);

        public void RequestInstance(ref IDomain instance) =>
            InstanceRequested?.Invoke(ref instance);

        public void RequestDeploymentAccess(ref IDomain instance, ref Deployment.Domain deployment) =>
            DeploymentAccessRequested?.Invoke(ref instance, ref deployment);

        public void RequestControlResolve(string controlId, ref IDomain instance, out IBase control)
        {
            control = new Unknown();
            ControlResolveRequested?.Invoke(controlId, ref instance, out control);
        }

        public void Process()
        {
            if (this.UpdateBlockIdStack.Count > 0)
            {
                if (!(this._Directive is Single single))
                    throw new System.Exception("update request container should be single!");

                single.Parse();

                IDirective result =
                    single.Children.Find(this.UpdateBlockIdStack.Peek());

                if (result == null)
                    return;

                single.Children.Clear();
                single.Children.Add(result);
            }

            this._Directive.Render(null);
        }

        public string Result => this._Directive.Result;
        public bool HasInlineError => this._Directive.HasInlineError;
    }
}