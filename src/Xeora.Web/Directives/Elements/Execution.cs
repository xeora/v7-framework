using System.Collections.Generic;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Execution : Directive, ILevelable, IBoundable, IHasBind
    {
        public Execution(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Execution, arguments)
        {
            this.Leveling = LevelingInfo.Create(rawValue);
            this.BoundDirectiveId = DirectiveHelper.CaptureBoundDirectiveId(rawValue);
            
            string[] controlValueSplitted = 
                rawValue.Split(':');
            this.Bind = Basics.Execution.Bind.Make(string.Join(":", controlValueSplitted, 1, controlValueSplitted.Length - 1));
        }

        public LevelingInfo Leveling { get; }
        public string BoundDirectiveId { get; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveId);
        public Basics.Execution.Bind Bind { get; }

        public override bool Searchable => false;
        public override bool CanAsync => false;

        public override void Parse()
        { }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            string uniqueId =
                string.IsNullOrEmpty(requesterUniqueId) ? this.UniqueId : requesterUniqueId;

            if (this.HasBound)
            {
                if (string.IsNullOrEmpty(requesterUniqueId))
                    return;

                this.Mother.Pool.GetByDirectiveId(this.BoundDirectiveId, out IEnumerable<IDirective> directives);

                if (directives == null) return;

                foreach (IDirective directive in directives)
                {
                    if (!(directive is INameable)) return;

                    string directiveId = ((INameable)directive).DirectiveId;
                    if (string.CompareOrdinal(directiveId, this.BoundDirectiveId) != 0) return;

                    if (directive.Status == RenderStatus.Rendered) continue;
                    
                    directive.Scheduler.Register(this.UniqueId);
                    return;
                }
            }

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.ExecuteBind(uniqueId);
        }

        private void ExecuteBind(string requesterUniqueId)
        {
            int level = this.Leveling.Level;
            IDirective leveledDirective = this;

            while (level > 0)
            {
                leveledDirective = leveledDirective.Parent;
                level--;

                if (leveledDirective != null) continue;
                
                leveledDirective = this;
                break;
            }
            
            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this.Bind.Parameters.Prepare(
                parameter => DirectiveHelper.RenderProperty(leveledDirective.Parent, parameter.Query, leveledDirective.Parent.Arguments, requesterUniqueId)
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.AssemblyCore.InvokeBind<object>(Helpers.Context.Request.Header.Method, this.Bind, Manager.ExecuterTypes.Other);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            if (invokeResult.Result is Basics.ControlResult.RedirectOrder redirectOrder)
            {
                Helpers.Context.AddOrUpdate("RedirectLocation", redirectOrder.Location);

                this.Deliver(RenderStatus.Rendered, string.Empty);

                return;
            }

            this.Deliver(
                RenderStatus.Rendered,
                Manager.AssemblyCore.GetPrimitiveValue(invokeResult.Result)
            );
        }
    }
}
