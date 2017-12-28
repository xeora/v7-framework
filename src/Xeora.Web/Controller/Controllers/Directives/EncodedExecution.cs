namespace Xeora.Web.Controller.Directive
{
    public class EncodedExecution : DirectiveWithChildren
    {
        public EncodedExecution(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.EncodedExecution, contentArguments)
        { }

        public override void Render(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            // EncodedExecution does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            string blockContent = contentDescription.Parts[0];

            this.Parse(blockContent);

            if (string.IsNullOrEmpty(blockContent))
                throw new Exception.EmptyBlockException();

            base.Render(requesterUniqueID);
        }

        public override void Build()
        {
            base.Build();

            if (this.IsUpdateBlockController)
            {
                this.RenderedValue =
                    string.Format(
                        "javascript:__XeoraJS.update('{0}', '{1}');",
                        this.UpdateBlockControlID,
                        Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.RenderedValue)
                    );

                return;
            }

            this.RenderedValue =
                string.Format(
                    "javascript:__XeoraJS.post('{0}');",
                    Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.RenderedValue)
                );
        }
    }
}
