using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class Mother : ControllerWithChildren, IMother
    {
        public event ParsingHandler ParseRequested;
        private ControllerPool _Pool;

        public Mother(int rawStartIndex, string rawValue, Basics.ControlResult.Message messageResult, string updateBlockControlID) :
            base(rawStartIndex, rawValue, ControllerTypes.Mother, null)
        {
            this._Pool = new ControllerPool();
            this.Scheduler = new ControllerSchedule(ref this._Pool);
            this.MessageResult = messageResult;
            this.ProcessingUpdateBlockControlID = updateBlockControlID;

            base.Mother = this;
        }

        public ControllerPool Pool => this._Pool;
        public ControllerSchedule Scheduler { get; private set; }
        public Basics.ControlResult.Message MessageResult { get; private set; }
        public string ProcessingUpdateBlockControlID { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            if (this.Parent != null)
                throw new Exception.HasParentException();

            if (!string.IsNullOrEmpty(this.ProcessingUpdateBlockControlID))
            {
                IController updateBlockController =
                    base.Find(this.ProcessingUpdateBlockControlID);

                if (updateBlockController != null)
                {
                    updateBlockController.Render(requesterUniqueID);
                    ((IHasChildren)updateBlockController).Build();

                    this.RenderedValue = updateBlockController.RenderedValue;
                }

                return;
            }

            this.Parse(this.RawValue);
            base.Render(requesterUniqueID);
            base.Build();
        }

        public void RequestParsing(string rawValue, ref ControllerCollection childrenContainer, ArgumentInfoCollection contentArguments) =>
            ParseRequested?.Invoke(rawValue, ref childrenContainer, contentArguments);
    }
}