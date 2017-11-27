namespace Xeora.Web.Controller.Directive
{
    public abstract class DirectiveWithChildren : ControllerWithChildren
    {
        protected DirectiveWithChildren(int rawStartIndex, string rawValue, DirectiveTypes directiveType, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, ControllerTypes.Directive, contentArguments)
        {
            this.DirectiveType = directiveType;
        }

        public DirectiveTypes DirectiveType { get; private set; }
    }
}