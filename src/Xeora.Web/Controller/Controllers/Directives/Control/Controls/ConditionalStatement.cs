using System.Text.RegularExpressions;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Controller.Directive.Control
{
    public class ConditionalStatement : ControlWithContent, IInstanceRequires, IBoundable
    {
        public event InstanceHandler InstanceRequested;

        public ConditionalStatement(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        { }

        public override IControl Clone() =>
            new ConditionalStatement(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            // ConditionalStatment does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            string contentTrue = contentDescription.Parts[0];
            string contentFalse = string.Empty;

            if (contentDescription.Parts.Count > 1)
                contentFalse = contentDescription.Parts[1];

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
                    string query = parameter.Query;

                    Match match =
                        _ParameterPointerRegEx.Match(parameter.Query);

                    if (match.Success)
                    {
                        int paramIndex =
                            int.Parse(match.Groups["index"].Value);

                        if (paramIndex >= this.Parameters.Length)
                            throw new Exception.FormatIndexOutOfRangeException();

                        query = this.Parameters[paramIndex];
                    }

                    Property property = new Property(0, query, (leveledController.Parent?.ContentArguments))
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

            Basics.Execution.InvokeResult<Basics.ControlResult.Conditional> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.Conditional>(Basics.Helpers.Context.Request.Header.Method, this.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);
            // ----

            if (invokeResult.Result != null)
            {
                switch (invokeResult.Result.Result)
                {
                    case Basics.ControlResult.Conditional.Conditions.True:
                        if (!string.IsNullOrEmpty(contentTrue))
                        {
                            if (!this.Leveling.ExecutionOnly)
                            {
                                this.ContentArguments.Replace(leveledController.ContentArguments);
                                // Just parse the children to be accessable in search
                                this.Parse(contentTrue);
                                this.RenderedValue =
                                    ControllerHelper.RenderSingleContent(contentTrue, leveledController, this.ContentArguments, requesterUniqueID);
                            }
                            else
                            {
                                // Just parse the children to be accessable in search
                                this.Parse(contentTrue);
                                this.RenderedValue =
                                    ControllerHelper.RenderSingleContent(contentTrue, this, this.ContentArguments, requesterUniqueID);
                            }
                        }

                        break;
                    case Basics.ControlResult.Conditional.Conditions.False:
                        if (!string.IsNullOrEmpty(contentFalse))
                        {
                            if (!this.Leveling.ExecutionOnly)
                            {
                                this.ContentArguments.Replace(leveledController.ContentArguments);
                                // Just parse the children to be accessable in search
                                this.Parse(contentFalse);
                                this.RenderedValue =
                                    ControllerHelper.RenderSingleContent(contentFalse, leveledController, this.ContentArguments, requesterUniqueID);
                            }
                            else
                            {
                                // Just parse the children to be accessable in search
                                this.Parse(contentFalse);
                                this.RenderedValue =
                                    ControllerHelper.RenderSingleContent(contentFalse, this, this.ContentArguments, requesterUniqueID);
                            }
                        }

                        break;
                    case Basics.ControlResult.Conditional.Conditions.Unknown:
                        // Reserved For Future Uses

                        break;
                }
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }

        public override void Build()
        {
            // Just override to bypass the base builder.
        }
    }
}