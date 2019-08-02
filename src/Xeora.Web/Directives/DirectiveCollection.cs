using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xeora.Web.Basics;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public class DirectiveCollection : List<IDirective>
    {
        private readonly SemaphoreSlim _SemaphoreSlim;
        private readonly ConcurrentQueue<int> _Queued;

        private readonly IMother _Mother;
        private readonly IDirective _Parent;

        public DirectiveCollection(IMother mother, IDirective parent)
        {
            this._SemaphoreSlim = new SemaphoreSlim(Configurations.Xeora.Service.Parallelism);
            this._Queued = new ConcurrentQueue<int>();

            this._Mother = mother;
            this._Parent = parent;
        }

        public new void Add(IDirective item)
        {
            item.Mother = this._Mother;
            item.Parent = this._Parent;
            
            this.AssignTemplateTree(ref item);

            this._Mother.Pool.Register(item);

            if (item is Control control)
                control.Load();

            this._Queued.Enqueue(this.Count);
            base.Add(item);
        }

        public new void AddRange(IEnumerable<IDirective> collection)
        {
            foreach (IDirective item in collection)
                this.Add(item);
        }

        private void AssignTemplateTree(ref IDirective item)
        {
            if (!string.IsNullOrEmpty(item.TemplateTree))
                return;

            if (item is INameable nameable)
                item.TemplateTree = nameable.DirectiveId;

            if (item is Template)
                return;
            
            item.TemplateTree = $"{item.Parent.TemplateTree}/{item.TemplateTree}";
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
                        d =>
                        {
                            this._SemaphoreSlim.Wait();
                            try
                            {
                                this.Render(currentHandlerId, requesterUniqueId, (IDirective) d);
                            }
                            finally
                            {
                                this._SemaphoreSlim.Release();
                            }
                        },
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
                // Analysis Calculation
                DateTime renderBegins = DateTime.Now;

                directive.Render(requesterUniqueId);

                if (directive.Parent != null)
                    directive.Parent.HasInlineError |= directive.HasInlineError;

                if (!Configurations.Xeora.Application.Main.PrintAnalysis) return;
                
                string analysisOutput = directive.UniqueId;
                switch (directive)
                {
                    case INameable nameable:
                        analysisOutput = $"{analysisOutput} - {nameable.DirectiveId}";
                        break;
                    case IHasBind hasBind:
                        analysisOutput = $"{analysisOutput} - {hasBind.Bind}";
                        break;
                }

                double totalMs = 
                    DateTime.Now.Subtract(renderBegins).TotalMilliseconds;
                Basics.Console.Push(
                    $"analysed - {directive.GetType().Name}",
                    $"{totalMs}ms {{{analysisOutput}}}",
                    string.Empty, false, groupId: Helpers.Context.UniqueId,
                    type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);
            }
            catch (Exception ex)
            {
                if (directive.Parent != null)
                    directive.Parent.HasInlineError = true;

                Tools.EventLogger.Log(ex);

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
                                !string.IsNullOrEmpty(exceptionString) ? string.Concat("<hr size='1px' />", exceptionString) : null
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
            if (directives == null) return null;
            
            foreach (IDirective directive in directives)
            {
                if (!directive.Searchable)
                    continue;

                if (!(directive is INameable))
                    continue;

                if (string.CompareOrdinal(((INameable)directive).DirectiveId, directiveId) == 0)
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