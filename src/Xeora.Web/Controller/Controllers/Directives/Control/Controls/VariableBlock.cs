using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Controller.Directive.Control
{
    public class VariableBlock : ControlWithContent, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;

        public VariableBlock(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        { }

        public override IControl Clone() =>
            new VariableBlock(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription = 
                new Global.ContentDescription(this.Value);

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

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this.Bind.Parameters.Prepare(
                (parameter) =>
                {
                    Property property = new Property(0, parameter.Query, (leveledController.Parent?.ContentArguments))
                    {
                        Mother = leveledController.Mother,
                        Parent = leveledController.Parent
                    };
                    property.InstanceRequested += (ref IDomain instance) => InstanceRequested?.Invoke(ref instance);
                    property.Setup();

                    property.Render(requesterUniqueID);

                    return property.ObjectResult;
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.VariableBlock> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.VariableBlock>(Basics.Helpers.Context.Request.Header.Method, this.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            if (invokeResult.Result == null)
                return;
            // ----

            if (invokeResult.Result.Message != null)
            {
                if (!contentDescription.HasMessageTemplate)
                    this.RenderedValue = invokeResult.Result.Message.Content;
                else
                {
                    this.ContentArguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this.ContentArguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.RenderedValue =
                        ControllerHelper.RenderSingleContent(
                            contentDescription.MessageTemplate, this, this.ContentArguments, requesterUniqueID);
                }

                this.Mother.Scheduler.Fire(this.ControlID);

                return;
            }

            if (!this.Leveling.ExecutionOnly)
                this.ContentArguments.Replace(leveledController.ContentArguments);

            if (invokeResult.Result != null)
            {
                foreach (string key in invokeResult.Result.Keys)
                    this.ContentArguments.AppendKeyWithValue(key, invokeResult.Result[key]);
            }

            // Just parse the children to be accessable in search
            string blockContent = contentDescription.Parts[0];

            this.Parse(blockContent);

            if (!this.Leveling.ExecutionOnly)
            {
                this.RenderedValue =
                    ControllerHelper.RenderSingleContent(blockContent, leveledController, this.ContentArguments, requesterUniqueID);
            }
            else
            {
                this.RenderedValue =
                    ControllerHelper.RenderSingleContent(blockContent, this, this.ContentArguments, requesterUniqueID);
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }

        public override void Build()
        {
            // Just override to bypass the base builder.
        }
    }
}
