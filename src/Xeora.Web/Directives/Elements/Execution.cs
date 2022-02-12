using System;
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
            
            string[] controlValueParts = 
                rawValue.Split(':');
            this.Bind = Basics.Execution.Bind.Make(string.Join(":", controlValueParts, 1, controlValueParts.Length - 1));
        }

        public LevelingInfo Leveling { get; }
        public string BoundDirectiveId { get; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveId);
        public Basics.Execution.Bind Bind { get; }

        public override bool Searchable => false;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => false;

        public override void Parse()
        {
            // Execution needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);
            
            this.Children = new DirectiveCollection(this.Mother, this);
        }

        public override bool PreRender()
        {
            if (this.HasBound)
            {
                this.Mother.Pool.GetByDirectiveId(this.BoundDirectiveId, out IEnumerable<IDirective> directives);

                if (directives == null) return false;

                foreach (IDirective directive in directives)
                {
                    if (!(directive is INameable)) return false;

                    string directiveId = ((INameable)directive).DirectiveId;
                    if (string.CompareOrdinal(directiveId, this.BoundDirectiveId) != 0) return false;

                    if (directive.Status == RenderStatus.Rendered) continue;
                    
                    directive.Scheduler.Register(this.UniqueId);
                    return false;
                }
            }

            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            return this.ExecuteBind();
        }

        public override void PostRender() => 
            this.Deliver(RenderStatus.Rendered, this.Result);

        private bool ExecuteBind()
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
                parameter =>
                {
                    Tuple<bool, object> result =
                        Directives.Property.Render(leveledDirective, parameter.Query);
                    return result.Item2;
                });

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.Executer.InvokeBind<object>(Helpers.Context.Request.Header.Method, this.Bind, Manager.ExecuterTypes.Other);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception);

            if (invokeResult.Result is Basics.ControlResult.RedirectOrder redirectOrder)
            {
                Helpers.Context.AddOrUpdate("RedirectLocation", redirectOrder.Location);

                // this.Children.Add(
                //    new Static(string.Empty));
                return true;
            }

            string result =
                Manager.Executer.GetPrimitiveValue(invokeResult.Result);
            
            if (!string.IsNullOrEmpty(result)) 
                this.Children.Add(new Static(result));
            
            return true;
        }
    }
}
