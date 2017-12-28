using Xeora.Web.Basics;

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

            string blockContent = contentDescription.Parts[0];

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
            this.Bind.PrepareProcedureParameters(
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

            Basics.Execution.BindInvokeResult<Basics.ControlResult.VariableBlock> bindInvokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.VariableBlock>(this.Bind, Manager.ExecuterTypes.Control);

            if (bindInvokeResult.Exception != null)
                throw new Exception.ExecutionException(bindInvokeResult.Exception.Message, bindInvokeResult.Exception.InnerException);
            // ----

            if (!this.Leveling.ExecutionOnly)
                this.ContentArguments.Replace(leveledController.ContentArguments);

            if (bindInvokeResult.Result != null)
            {
                foreach (string key in bindInvokeResult.Result.Keys)
                    this.ContentArguments.AppendKeyWithValue(key, bindInvokeResult.Result[key]);
            }

            // Just parse the children to be accessable in search
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
