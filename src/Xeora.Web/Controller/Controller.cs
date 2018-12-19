using System;

namespace Xeora.Web.Controller
{
    public abstract class Controller : IController
    {
        protected Controller(int rawStartIndex, string rawValue, ControllerTypes controllerType, Global.ArgumentInfoCollection contentArguments)
        {
            this.UniqueID = Guid.NewGuid().ToString();

            this.Mother = null;
            this.Parent = null;

            this.RawValue = rawValue;
            this.RawStartIndex = rawStartIndex;

            this.Value = rawValue;

            // Remove block signs, this value must not be null
            if (!string.IsNullOrEmpty(this.Value) &&
                this.Value.Length > 2 &&
                this.Value[0] == '$' &&
                this.Value[this.Value.Length - 1] == '$')
            {
                this.Value = this.Value.Substring(1, this.Value.Length - 2);
                this.Value = this.Value.Trim();
            }
            // !--

            this.ControllerType = controllerType;
            this.ContentArguments = contentArguments;
            if (this.ContentArguments == null)
                this.ContentArguments = new Global.ArgumentInfoCollection();

            this.RenderedValue = string.Empty;
        }

        public string UniqueID { get; private set; }
        public System.Exception Exception { get; set; }

        public IMother Mother { get; set; }
        public IController Parent { get; set; }

        public string RawValue { get; private set; }
        public int RawStartIndex { get; private set; }
        public int RawEndIndex => (this.RawStartIndex + this.RawLength);
        public int RawLength => this.RawValue.Length;

        public string Value { get; private set; }

        public ControllerTypes ControllerType { get; private set; }
        public Global.ArgumentInfoCollection ContentArguments { get; private set; }

        public string UpdateBlockControlID { get; private set; }
        public bool IsUpdateBlockController => !string.IsNullOrEmpty(this.UpdateBlockControlID);

        public string RenderedValue { get; protected set; }
        public bool HasInlineError { get; protected set; }
        public bool IsRendered => !string.IsNullOrEmpty(this.RenderedValue);

        public void Setup()
        {
            if (this.Mother != null)
                this.Mother.Pool.Register(this);

            IController controller = this;

            while (controller.Parent != null) 
            {
                controller = controller.Parent;

                if (controller is Directive.UpdateBlock)
                {
                    this.UpdateBlockControlID = ((Directive.UpdateBlock)controller).ControlID;

                    break;
                }
            } 
        }

        public abstract void Render(string requesterUniqueID);
    }
}