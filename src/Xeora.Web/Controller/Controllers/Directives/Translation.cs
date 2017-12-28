using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class Translation : Directive, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;

        public Translation(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.Translation, contentArguments)
        { }

        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            string translationID = this.Value.Split(':')[1];

            IDomain instance = null;
            InstanceRequested(ref instance);

            IDomain examingInstance = instance;
            string translationText = string.Empty;
            do
            {
                translationText = examingInstance.Language.Get(translationID);

                if (string.IsNullOrEmpty(translationText))
                    examingInstance = examingInstance.Parent;
            } while (examingInstance != null && string.IsNullOrEmpty(translationText));

            this.RenderedValue = translationText;
        }
    }
}