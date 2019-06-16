using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Execution : Directive, ILevelable, IBoundable
    {
        private readonly string _RawValue;
        private bool _Queued;

        public Execution(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Execution, arguments)
        {
            this._RawValue = rawValue;

            this.Leveling = LevelingInfo.Create(rawValue);
            this.BoundDirectiveID = DirectiveHelper.CaptureBoundDirectiveID(rawValue);
        }

        public LevelingInfo Leveling { get; private set; }
        public string BoundDirectiveID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveID);

        public override bool Searchable => false;
        public override bool CanAsync => false;

        public override void Parse()
        { }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            string uniqueID = this.UniqueID;

            if (this.HasBound)
            {
                if (string.IsNullOrEmpty(requesterUniqueID))
                    return;

                this.Mother.Pool.GetInto(requesterUniqueID, out IDirective directive);

                if (directive == null ||
                    (directive is INamable &&
                        string.Compare(((INamable)directive).DirectiveID, this.BoundDirectiveID) != 0)
                    )
                {
                    if (!this._Queued)
                    {
                        this._Queued = true;
                        this.Mother.Scheduler.Register(this.BoundDirectiveID, this.UniqueID);
                    }

                    return;
                }

                uniqueID = requesterUniqueID;
            }

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.ExecuteBind(uniqueID);
        }

        private void ExecuteBind(string requesterUniqueID)
        {
            string[] controlValueSplitted = 
                this._RawValue.Split(':');

            int level = this.Leveling.Level;
            IDirective leveledDirective = this;

            while (level > 0)
            {
                leveledDirective = leveledDirective.Parent;
                level--;

                if (leveledDirective == null)
                {
                    leveledDirective = this;
                    break;
                }
            }

            Basics.Execution.Bind bind =
                Basics.Execution.Bind.Make(string.Join(":", controlValueSplitted, 1, controlValueSplitted.Length - 1));

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            bind.Parameters.Prepare(
                (parameter) => DirectiveHelper.RenderProperty(leveledDirective.Parent, parameter.Query, leveledDirective.Parent.Arguments, requesterUniqueID)
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.AssemblyCore.InvokeBind<object>(Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Other);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            if (invokeResult.Result != null && invokeResult.Result is Basics.ControlResult.RedirectOrder)
            {
                Helpers.Context.AddOrUpdate("RedirectLocation",
                    ((Basics.ControlResult.RedirectOrder)invokeResult.Result).Location);

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
