using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class HashCodePointedTemplate : Directive
    {
        public HashCodePointedTemplate(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments) : 
            base(rawStartIndex, rawValue, DirectiveTypes.HashCodePointedTemplate, contentArguments)
        { }

        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            string[] controlValueSplitted = this.Value.Split(':');
            this.RenderedValue = string.Format("{0}/{1}", Helpers.Context.HashCode, controlValueSplitted[1]);
        }
    }
}