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

            if (string.IsNullOrEmpty(requesterUniqueID))
                return;

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

            Basics.Execution.Bind bind =
                Basics.Execution.Bind.Make(string.Join(":", controlValueSplitted, 1, controlValueSplitted.Length - 1));

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            bind.Parameters.Prepare(
                (parameter) =>
                {
                    Property property = new Property(0, parameter.Query, (leveledController.Parent == null ? null : leveledController.Parent.ContentArguments));
                    property.Mother = leveledController.Mother;
                    property.Parent = leveledController.Parent;
                    property.InstanceRequested += (ref Basics.Domain.IDomain instance) => InstanceRequested?.Invoke(ref instance);
                    property.Setup();

                    property.Render(requesterUniqueID);

                    return property.ObjectResult;
                }
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.AssemblyCore.InvokeBind<object>(Basics.Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Other);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            if (invokeResult.Result != null && invokeResult.Result is Basics.ControlResult.RedirectOrder)
            {
                Helpers.Context.AddOrUpdate("RedirectLocation",
                    ((Basics.ControlResult.RedirectOrder)invokeResult.Result).Location);

                this.RenderedValue = string.Empty;

                return;
            }

            this.RenderedValue = Manager.AssemblyCore.GetPrimitiveValue(invokeResult.Result);
        }
    }
}
