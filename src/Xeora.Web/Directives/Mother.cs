using System;
using System.Collections.Generic;
using System.Linq;
using Xeora.Web.Application.Controls;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Global;
using Single = Xeora.Web.Directives.Elements.Single;

namespace Xeora.Web.Directives
{
    public class Mother : IMother
    {
        private readonly IDirective _Directive;

        public event ParsingHandler ParseRequested;
        public event InstanceHandler InstanceRequested;
        public event DeploymentAccessHandler DeploymentAccessRequested;
        public event ControlResolveHandler ControlResolveRequested;

        public Mother(IDirective directive, Basics.ControlResult.Message messageResult, IReadOnlyCollection<string> updateBlockIdList)
        {
            this.Pool = new DirectivePool();
            

            this.MessageResult = messageResult;
            this.RequestedUpdateBlockIds = 
                new List<string>();
            if (updateBlockIdList != null && updateBlockIdList.Count > 0)
                this.RequestedUpdateBlockIds.AddRange(updateBlockIdList);
            
            this._Directive = directive;
            this._Directive.Mother = this;
        }

        public Mother(string xeoraContent, Basics.ControlResult.Message messageResult, IReadOnlyCollection<string> updateBlockIdStack) :
            this(new Single(xeoraContent, null), messageResult, updateBlockIdStack)
        { }

        public DirectivePool Pool { get; }

        public Basics.ControlResult.Message MessageResult { get; }
        public List<string> RequestedUpdateBlockIds { get; }

        public void RequestParsing(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments) =>
            ParseRequested?.Invoke(rawValue, ref childrenContainer, arguments);

        public void RequestInstance(out IDomain instance)
        {
            instance = null; 
            InstanceRequested?.Invoke(out instance);
        }

        public void RequestDeploymentAccess(ref IDomain instance, out Deployment.Domain deployment)
        {
            deployment = null;
            DeploymentAccessRequested?.Invoke(ref instance, out deployment);
        }

        public void RequestControlResolve(string controlId, ref IDomain instance, out IBase control)
        {
            control = new Unknown();
            ControlResolveRequested?.Invoke(controlId, ref instance, out control);
        }

        public void Process()
        {
            if (this.RequestedUpdateBlockIds.Count > 0)
            {
                if (!(this._Directive is Single single))
                    throw new Exception("update request container should be single!");

                single.Parse();

                IDirective result =
                    single.Children.Find(this.RequestedUpdateBlockIds.Last());

                if (result == null)
                    return;

                single.Children.Clear();
                single.Children.Add(result);
            }

            this._Directive.Render(null);
        }

        public string Result => this._Directive.Result;
        public bool HasInlineError => this._Directive.HasInlineError;

        public static string CreateErrorOutput(Exception exception)
        {
            if (!Configurations.Xeora.Application.Main.Debugging) return string.Empty;
            
            string exceptionString = string.Empty;
            do
            {
                exceptionString =
                    string.Format(
                        @"<div align='left' style='border: solid 1px #660000; background-color: #ffffff'>
                                <div align='left' style='font-weight: bolder; color:#ffffff; background-color:#cc0000; padding: 4px;'>{0}</div>
                                <br/>
                                <div align='left' style='padding: 4px'>{1}{2}</div>
                              </div>",
                        exception.Message,
                        exception.Source,
                        !string.IsNullOrEmpty(exceptionString) ? string.Concat("<hr size='1px' />", exceptionString) : null
                    );

                exception = exception.InnerException;
            } while (exception != null);

            return exceptionString;
        }
    }
}