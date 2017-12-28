namespace Xeora.Web.Controller.Directive
{
    public class Single : ControllerWithChildren
    {
        public Single(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, ControllerTypes.Generic, contentArguments)
        { }

        public override void Render(string requesterUniqueID)
        {
            this.Parse(this.RawValue);
            base.Render(requesterUniqueID);
            base.Build();
        }
    }
}