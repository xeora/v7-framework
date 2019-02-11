using System.Text;
using Xeora.Web.Controller.Directive;

namespace Xeora.Web.Controller
{
    public abstract class ControllerWithChildren : Controller, IHasChildren
    {
        protected ControllerWithChildren(int rawStartIndex, string rawValue, ControllerTypes controllerType, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, controllerType, contentArguments)
        {
            this.Children = new ControllerCollection(this);
        }

        public ControllerCollection Children { get; private set; }

        protected void Parse(string rawValue)
        {
            if (this.Mother == null)
                return;

            if (this.Children.Count > 0)
                return;

            ControllerCollection children = this.Children;
            this.Mother.RequestParsing(rawValue, ref children, this.ContentArguments);
        }

        public override void Render(string requesterUniqueID) =>
            this.Children.Render(requesterUniqueID);

        public virtual IController Find(string controlID)
        {
            string content = this.RawValue;

            if (this is Template)
                content = this.RenderedValue;

            this.Parse(content);

            foreach (IController child in this.Children)
            {
                if (child is INamable && child is UpdateBlock &&
                    string.Compare(((INamable)child).ControlID, controlID) == 0)
                    return child;

                if (child is IHasChildren)
                {
                    IController innerChild =
                        ((IHasChildren)child).Find(controlID);

                    if (innerChild != null)
                        return innerChild;
                }
                else if (child is Directive.Control.ConditionalStatement)
                {
                    // ConditionalStatement should be rendered, however it would be bound or
                    // depending on some other control value. That's why, its parent should be rendered
                    child.Parent.Render(this.UniqueID);
                }
            }

            return null;
        }

        public virtual void Build()
        {
            StringBuilder builder = new StringBuilder();

            foreach (IController controller in this.Children)
            {
                try
                {
                    if (controller.Exception != null)
                        throw controller.Exception;

                    if (controller is IHasChildren)
                        ((IHasChildren)controller).Build();

                    builder.Append(controller.RenderedValue);

                    this.HasInlineError |= controller.HasInlineError;
                }
                catch (System.Exception ex)
                {
                    Helper.EventLogger.Log(ex);

                    if (Basics.Configurations.Xeora.Application.Main.Debugging)
                    {
                        string exceptionString = null;
                        do
                        {
                            exceptionString =
                                string.Format(
                                    "<div align='left' style='border: solid 1px #660000; background-color: #FFFFFF'><div align='left' style='font-weight: bolder; color:#FFFFFF; background-color:#CC0000; padding: 4px;'>{0}</div><br><div align='left' style='padding: 4px'>{1}{2}</div></div>",
                                    ex.Message,
                                    ex.Source,
                                    (!string.IsNullOrEmpty(exceptionString) ? string.Concat("<hr size='1px' />", exceptionString) : null)
                                );

                            ex = ex.InnerException;
                        } while (ex != null);

                        builder.Append(exceptionString);
                    }

                    this.HasInlineError = true;
                }
            }

            this.RenderedValue = builder.ToString();
        }
    }
}