using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive.Control
{
    public abstract class ControlWithContent : DirectiveWithChildren, IControl
    {
        public ControlWithContent(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments, ControlSettings settings) : 
            base(rawStartIndex, rawValue, DirectiveTypes.Control, contentArguments)
        {
            this.Settings = settings;

            this.Type = settings.Type;

            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
            this.BoundControlID = DirectiveHelper.CaptureBoundControlID(this.Value);
            this.Leveling = LevelingInfo.Create(this.Value);

            this.Security = settings.Security;
            this.Bind = settings.Bind;
            this.Attributes = settings.Attributes;
        }

        public string ControlID { get; private set; }
        public string BoundControlID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundControlID);
        public LevelingInfo Leveling { get; private set; }

        public ControlSettings Settings { get; private set; }

        public ControlTypes Type { get; private set; }
        public SecurityInfo Security { get; private set; }
        public Basics.Execution.BindInfo Bind { get; protected set; }
        public AttributeInfoCollection Attributes { get; private set; }

        protected abstract void RenderControl(string requesterUniqueID);
        public abstract IControl Clone();

        public override void Render(string requesterUniqueID)
        {
            if (!this.HasBound)
            {
                this.RenderControl(requesterUniqueID);

                return;
            }

            IController controller = null;
            this.Mother.Pool.GetInto(requesterUniqueID, out controller);

            if (controller != null &&
                controller is INamable &&
                string.Compare(((INamable)controller).ControlID, this.BoundControlID) == 0)
            {
                this.RenderControl(requesterUniqueID);

                return;
            }

            this.Mother.Scheduler.Register(this.BoundControlID, this.UniqueID);
        }
    }
}