using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Service.Workers;

namespace Xeora.Web.Directives
{
    public class DirectiveCollection : List<IDirective>
    {
        private readonly ConcurrentQueue<int> _Queued;

        private readonly IMother _Mother;
        private readonly IDirective _Parent;

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
                item.TemplateTree = $"/{nameable.DirectiveId}";

            if (item is Template)
                return;
            
            item.TemplateTree = $"{item.Parent.TemplateTree}{item.TemplateTree}";
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
            string handlerId =
                Helpers.CurrentHandlerId;
            Bulk bulk = Factory.CreateBulk();

            int queueLength = 
                this._Queued.Count;

            while (this._Queued.TryDequeue(out int index))
            {
                IDirective directive = this[index];
                
                if (!directive.CanAsync || queueLength == 1)
                {
                    this.Render(directive);
                    continue;
                }

                ActionType actionType = directive switch
                {
                    Translation or Static or ReplaceableTranslation or Elements.Property => ActionType.Attached,
                    AsyncGroup or ControlAsync or MessageBlock or SingleAsync => ActionType.External,
                    _ => ActionType.None
                };

                if (actionType == ActionType.None) continue;

                bulk.Add(
                    d =>
                    {
                        Helpers.AssignHandlerId(handlerId);
                        this.Render((IDirective)d);
                    },
                    directive,
                    actionType
                );
            }

            bulk.Process();

            this.Deliver();
        }
        
        private void Deliver()
        {
            try
            {
                StringBuilder resultContainer = new StringBuilder();

                foreach (IDirective directive in this)
                    resultContainer.Append(directive.Result);

                this._Parent.Deliver(RenderStatus.Rendering, resultContainer.ToString());
            }
            catch (Exception ex)
            {
                DirectiveCollection.HandleError(ex, this._Parent);
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
                
                this.CreateAnalysisReport(renderBegins, directive);
            }
            catch (Exception ex)
            {
                DirectiveCollection.HandleError(ex, directive);
            }
        }

        private void CreateAnalysisReport(DateTime renderBegins, IDirective directive)
        {
            if (!Configurations.Xeora.Application.Main.PrintAnalysis) return;
            
            string analysisOutput = directive switch
            {
                INameable nameable => $"{directive.UniqueId} - {nameable.DirectiveId}",
                IHasBind hasBind => $"{directive.UniqueId} - {hasBind.Bind}",
                _ => directive.UniqueId
            };

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

        private static void HandleError(Exception exception, IDirective directive)
        {
            if (directive.Parent != null)
                directive.Parent.HasInlineError = true;

            Basics.Console.Push("Execution Exception...", exception.Message, exception.ToString(), false, true, type: Basics.Console.Type.Error);

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