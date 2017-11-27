using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive
{
    public class InLineStatement : DirectiveWithChildren, IInstanceRequires, INamable, IBoundable
    {
        private bool _NoCache = false;
        public event InstanceHandler InstanceRequested;

        public InLineStatement(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.InLineStatement, contentArguments)
        {
            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
            this.BoundControlID = DirectiveHelper.CaptureBoundControlID(this.Value);
        }

        public string ControlID { get; private set; }
        public string BoundControlID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundControlID);

        public override void Render(string requesterUniqueID)
        {
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
            Global.ContentDescription contentDescription = 
                new Global.ContentDescription(this.Value);

            string blockContent = contentDescription.Parts[0];

            string noCacheMarker = "!NOCACHE";

            if (blockContent.IndexOf(noCacheMarker) == 0)
            {
                this._NoCache = true;
                blockContent = blockContent.Substring(noCacheMarker.Length);
            }
            blockContent = blockContent.Trim();

            if (string.IsNullOrEmpty(blockContent))
                throw new Exception.EmptyBlockException();

            // InLineStatement does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            this.Parse(blockContent);
            base.Render(requesterUniqueID);
        }

        public override void Build()
        {
            base.Build();

            IDomain instance = null;
            InstanceRequested(ref instance);

            object methodResultInfo =
                Manager.AssemblyCore.ExecuteStatement(instance.IDAccessTree, this.ControlID, this.RenderedValue, this._NoCache);

            if (methodResultInfo != null && methodResultInfo is System.Exception)
                throw new Exception.ExecutionException(((System.Exception)methodResultInfo).Message, ((System.Exception)methodResultInfo).InnerException);

            if (methodResultInfo != null)
            {
                string renderResult = string.Empty;

                if (methodResultInfo is Basics.ControlResult.RedirectOrder)
                    Helpers.Context.AddOrUpdate("RedirectLocation", ((Basics.ControlResult.RedirectOrder)methodResultInfo).Location);
                else
                    renderResult = Basics.Execution.GetPrimitiveValue(methodResultInfo);

                this.RenderedValue = renderResult;

                return;
            }

            this.RenderedValue = string.Empty;
        }
    }
}