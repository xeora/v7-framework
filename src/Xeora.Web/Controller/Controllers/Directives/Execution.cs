using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive
{
    public class Execution : Directive, ILevelable, IBoundable, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;

        public Execution(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.Execution, contentArguments)
        {
            this.Leveling = LevelingInfo.Create(this.Value);
            this.BoundControlID = DirectiveHelper.CaptureBoundControlID(this.Value);
        }

        public LevelingInfo Leveling { get; private set; }
        public string BoundControlID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundControlID);

        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            if (!this.HasBound)
            {
                this.RenderInternal(requesterUniqueID);

                return;
            }

            IController controller = null;
            this.Mother.Pool.GetInto(requesterUniqueID, out controller);

            if (controller != null &&
                controller is INamable &&
                string.Compare(((INamable)controller).ControlID, this.BoundControlID) == 0)
            {
                this.RenderInternal(requesterUniqueID);

                return;
            }

            this.Mother.Scheduler.Register(this.BoundControlID, this.UniqueID);
        }

        private void RenderInternal(string requesterUniqueID)
        {
            string[] controlValueSplitted = this.Value.Split(':');

            // Call Related Function and Exam It
            IController leveledController = this;
            int level = this.Leveling.Level;

            do
            {
                if (level == 0)
                    break;

                leveledController = leveledController.Parent;

                if (leveledController is Renderless)
                    leveledController = leveledController.Parent;

                level -= 1;
            } while (leveledController != null);

            Basics.Execution.BindInfo bindInfo =
                Basics.Execution.BindInfo.Make(string.Join(":", controlValueSplitted, 1, controlValueSplitted.Length - 1));

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            bindInfo.PrepareProcedureParameters(
                new Basics.Execution.BindInfo.ProcedureParser(
                    (ref Basics.Execution.BindInfo.ProcedureParameter procedureParameter) =>
                    {
                        Property property = new Property(0, procedureParameter.Query, (leveledController.Parent == null ? null : leveledController.Parent.ContentArguments));
                        property.Mother = leveledController.Mother;
                        property.Parent = leveledController.Parent;
                        property.InstanceRequested += (ref IDomain instance) => InstanceRequested(ref instance);
                        property.Setup();

                        property.Render(requesterUniqueID);

                        procedureParameter.Value = property.ObjectResult;
                    }
                )
            );

            Basics.Execution.BindInvokeResult<object> bindInvokeResult =
                Manager.AssemblyCore.InvokeBind<object>(bindInfo, Manager.ExecuterTypes.Other);

            if (bindInvokeResult.Exception != null)
                throw new Exception.ExecutionException(bindInvokeResult.Exception.Message, bindInvokeResult.Exception.InnerException);

            if (bindInvokeResult.Result != null && bindInvokeResult.Result is Basics.ControlResult.RedirectOrder)
            {
                Helpers.Context.AddOrUpdate("RedirectLocation",
                    ((Basics.ControlResult.RedirectOrder)bindInvokeResult.Result).Location);

                this.RenderedValue = string.Empty;

                return;
            }

            this.RenderedValue = Basics.Execution.GetPrimitiveValue(bindInvokeResult.Result);
        }
    }
}
