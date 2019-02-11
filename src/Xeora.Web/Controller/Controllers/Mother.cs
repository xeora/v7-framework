using System.Collections.Generic;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class Mother : ControllerWithChildren, IMother
    {
        public event ParsingHandler ParseRequested;
        private readonly ControllerPool _Pool;

        public Mother(int rawStartIndex, string rawValue, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack) :
            base(rawStartIndex, rawValue, ControllerTypes.Mother, null)
        {
            this._Pool = new ControllerPool();
            this.Scheduler = new ControllerSchedule(ref this._Pool);
            this.UpdateBlockControlIDStack = new Stack<string>();

            this.MessageResult = messageResult;
            if (updateBlockControlIDStack != null && updateBlockControlIDStack.Length > 0)
                foreach(string updateBlockControlID in updateBlockControlIDStack)
                    this.UpdateBlockControlIDStack.Push(updateBlockControlID);

            base.Mother = this;
        }

        public ControllerPool Pool => this._Pool;
        public ControllerSchedule Scheduler { get; private set; }
        public Basics.ControlResult.Message MessageResult { get; private set; }
        public Stack<string> UpdateBlockControlIDStack { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            if (this.Parent != null)
                throw new Exception.HasParentException();

            if (this.UpdateBlockControlIDStack.Count > 0)
            {
                IController updateBlockController =
                    base.Find(this.UpdateBlockControlIDStack.Peek());

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