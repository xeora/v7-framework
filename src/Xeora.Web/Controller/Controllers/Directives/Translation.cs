using Xeora.Web.Basics.Domain;
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
            InstanceRequested?.Invoke(ref instance);

            this.RenderedValue = instance.Languages.Current.Get(translationID);
        }
    }
}