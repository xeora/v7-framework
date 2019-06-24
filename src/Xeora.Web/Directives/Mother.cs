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

        public Mother(IDirective directive, Basics.ControlResult.Message messageResult, string[] updateBlockIDStack)
        {
            this._Pool = new DirectivePool();
            this.UpdateBlockIDStack = new Stack<string>();

            this.MessageResult = messageResult;
            if (updateBlockIDStack != null && updateBlockIDStack.Length > 0)
                foreach (string updateBlockControlID in updateBlockIDStack)
                    this.UpdateBlockIDStack.Push(updateBlockControlID);

            this._Directive = directive;
            this._Directive.Mother = this;
        }

        public Mother(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockIDStack) :
            this(new Single(xeoraContent, null), messageResult, updateBlockIDStack)
        { }

        public DirectivePool Pool => this._Pool;
        public Basics.ControlResult.Message MessageResult { get; private set; }
        public Stack<string> UpdateBlockIDStack { get; private set; }

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

        public void Process()
        {
            if (this.UpdateBlockIDStack.Count > 0)
            {
                if (!(this._Directive is Single single))
                    throw new System.Exception("update request container should be single!");

                single.Parse();

                IDirective result =
                    single.Children.Find(this.UpdateBlockIDStack.Peek());

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