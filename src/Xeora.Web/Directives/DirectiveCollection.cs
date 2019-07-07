using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xeora.Web.Basics;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public class DirectiveCollection : List<IDirective>
    {
        private readonly ConcurrentQueue<int> _Queued;

        private readonly IMother _Mother;
        private IDirective _Parent;

        public DirectiveCollection(IMother mother, IDirective parent)
        {
            this._Queued = new ConcurrentQueue<int>();

            this._Mother = mother;
            this._Parent = parent;
        }

        public new void Add(IDirective item)
        {
            item.Mother = this._Mother;
            item.Parent = this._Parent;

            this._Mother.Pool.Register(item);

            if (item is Control)
                ((Control)item).Load();

            this._Queued.Enqueue(this.Count);
            base.Add(item);
        }

        public new void AddRange(IEnumerable<IDirective> collection)
        {
            foreach (IDirective item in collection)
                this.Add(item);
        }

        public void Render(string requesterUniqueId)
        {
            string currentHandlerId = Helpers.CurrentHandlerId;

            List<Task> tasks = new List<Task>();

            while (this._Queued.TryDequeue(out int index))
            {
                IDirective directive = this[index];

                if (!directive.CanAsync)
                {
                    this.Render(currentHandlerId, requesterUniqueId, directive);
                    continue;
                }

                tasks.Add(
                    Task.Factory.StartNew(
                        (d) => this.Render(currentHandlerId, requesterUniqueId, (IDirective)d),
                        directive,
                        TaskCreationOptions.DenyChildAttach
                    )
                );
            }

            if (tasks.Count > 0)
                Task.WaitAll(tasks.ToArray());

            StringBuilder resultContainer = new StringBuilder();

            foreach (IDirective directive in this)
                resultContainer.Append(directive.Result);

            this._Parent.Deliver(RenderStatus.Rendering, resultContainer.ToString());
        }

        private void Render(string handlerId, string requesterUniqueId, IDirective directive)
        {
            Helpers.AssignHandlerId(handlerId);

            try
            {
                // Analytics Calculator
                DateTime renderBegins = DateTime.Now;

                directive.Render(requesterUniqueId);

                if (directive.Parent != null)
                    directive.Parent.HasInlineError |= directive.HasInlineError;

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    string analyticOutput = directive.UniqueId;
                    if (directive is INamable)
                        analyticOutput = string.Format("{0} - {1}", analyticOutput, ((INamable)directive).DirectiveId);
                    Basics.Console.Push(
                        string.Format("analytic - {0}", directive.GetType().Name),
                        string.Format("{0}ms {{{1}}}", DateTime.Now.Subtract(renderBegins).TotalMilliseconds, analyticOutput),
                        string.Empty, false);
                }
            }
            catch (System.Exception ex)
            {
                if (directive.Parent != null)
                    directive.Parent.HasInlineError = true;

                Helper.EventLogger.Log(ex);

                if (Configurations.Xeora.Application.Main.Debugging)
                {
                    string exceptionString = string.Empty;
                    do
                    {
                        exceptionString =
                            string.Format(
                                @"<div align='left' style='border: solid 1px #660000; background-color: #FFFFFF'>
                                    <div align='left' style='font-weight: bolder; color:#FFFFFF; background-color:#CC0000; padding: 4px;'>{0}</div>
                                    <br/>
                                    <div align='left' style='padding: 4px'>{1}{2}</div>
                                  </div>",
                                ex.Message,
                                ex.Source,
                                (!string.IsNullOrEmpty(exceptionString) ? string.Concat("<hr size='1px' />", exceptionString) : null)
                            );

                        ex = ex.InnerException;
                    } while (ex != null);

                    directive.Deliver(RenderStatus.Rendered, exceptionString);
                }
                else
                    directive.Deliver(RenderStatus.Rendered, string.Empty);
            }
        }

        public IDirective Find(string directiveId) => 
            this.Find(this, directiveId);

        private IDirective Find(DirectiveCollection directives, string directiveId)
        {
            foreach (IDirective directive in directives)
            {
                if (!directive.Searchable)
                    continue;

                if (!(directive is INamable))
                    continue;

                if (string.Compare(((INamable)directive).DirectiveId, directiveId) == 0)
                    return directive;

                if (directive is Control control)
                {
                    switch (control.Type)
                    {
                        case Basics.Domain.Control.ControlTypes.ConditionalStatement:
                        case Basics.Domain.Control.ControlTypes.VariableBlock:
                            directive.Render(directives._Parent?.UniqueId);

                            break;
                    }
                }
                else
                {
                    switch (directive.Type)
                    {
                        case DirectiveTypes.PermissionBlock:
                            directive.Render(directive.Parent?.UniqueId);

                            break;
                        default:
                            directive.Parse();

                            break;
                    }
                }

                DirectiveCollection children =
                    ((IHasChildren)directive).Children;

                IDirective result =
                    this.Find(children, directiveId);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}