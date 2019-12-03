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
            this._SemaphoreSlim = 
                new SemaphoreSlim(Configurations.Xeora.Service.Parallelism);
            this._Queued = new ConcurrentQueue<int>();

            this._Mother = mother;
            this._Parent = parent;
        }

        public new void Add(IDirective item)
        {
            item.Mother = this._Mother;
            item.Parent = this._Parent;
            
            DirectiveCollection.AssignTemplateTree(ref item);
            DirectiveCollection.AssignUpdateBlockIds(ref item);

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

        private static void AssignTemplateTree(ref IDirective item)
        {
            if (!string.IsNullOrEmpty(item.TemplateTree))
                return;

            if (item is INameable nameable)
                item.TemplateTree = nameable.DirectiveId;

            if (item is Template)
                return;
            
            item.TemplateTree = $"{item.Parent.TemplateTree}/{item.TemplateTree}";
        }
        
        private static void AssignUpdateBlockIds(ref IDirective item)
        {
            string[] itemUpdateBlockIds = 
                item.UpdateBlockIds.ToArray();
            item.UpdateBlockIds.Clear();
            
            item.UpdateBlockIds.AddRange(
                item.Parent.UpdateBlockIds.ToArray());
            item.UpdateBlockIds.AddRange(itemUpdateBlockIds);
        }

        public void Render()
        {
            string currentHandlerId = 
                Helpers.CurrentHandlerId;
            List<Task> tasks = new List<Task>();

            while (this._Queued.TryDequeue(out int index))
            {
                IDirective directive = this[index];

                if (!directive.CanAsync)
                {
                    this.Render(directive);
                    continue;
                }

                tasks.Add(
                    Task.Factory.StartNew(
                        s =>
                        {
                            Tuple<string, IDirective> @params = 
                                (Tuple<string, IDirective>) s;

                            string handlerId = 
                                @params.Item1;
                            IDirective d = @params.Item2;
                            
                            Helpers.AssignHandlerId(handlerId);
                            
                            this._SemaphoreSlim.Wait();
                            try
                            {
                                this.Render(d);
                            }
                            finally
                            {
                                this._SemaphoreSlim.Release();
                            }
                        },
                        new Tuple<string, IDirective>(currentHandlerId, directive)
                    )
                );
            }

            this.Deliver(tasks.ToArray());
        }

        private void Deliver(Task[] tasks)
        {
            try
            {
                if (tasks.Length > 0)
                    Task.WaitAll(tasks);
                
                StringBuilder resultContainer = new StringBuilder();

                foreach (IDirective directive in this)
                    resultContainer.Append(directive.Result);

                this._Parent.Deliver(RenderStatus.Rendering, resultContainer.ToString());
            }
            catch (Exception ex)
            {
                this.HandleError(ex, this._Parent);
            }
        }

        private void Render(IDirective directive)
        {
            try
            {
                // Analysis Calculation
                DateTime renderBegins = DateTime.Now;
                
                directive.Render();

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
                this._Mother.AnalysisBulk.AddOrUpdate(
                    directive.Type, 
                    new Tuple<int, double>(1, totalMs), 
                    (k, v) => new Tuple<int, double>(v.Item1 + 1, v.Item2 + totalMs));
                Basics.Console.Push(
                    $"analysed - {directive.GetType().Name}",
                    $"{totalMs}ms {{{analysisOutput}}}",
                    string.Empty, false, groupId: Helpers.Context.UniqueId,
                    type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);
            }
            catch (Exception ex)
            {
                this.HandleError(ex, directive);
            }
        }

        private void HandleError(Exception exception, IDirective directive)
        {
            if (directive.Parent != null)
                directive.Parent.HasInlineError = true;

            Basics.Console.Push("Execution Exception...", exception.Message, exception.StackTrace, false, true, type: Basics.Console.Type.Error);

            directive.Deliver(RenderStatus.Rendered, Mother.CreateErrorOutput(exception));
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
                            directive.Render();

                            break;
                    }
                }
                else
                {
                    switch (directive.Type)
                    {
                        case DirectiveTypes.PermissionBlock:
                            directive.Render();

                            break;
                        default:
                            directive.Parse();

                            break;
                    }
                }

                IDirective result =
                    this.Find(directive.Children, directiveId);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}