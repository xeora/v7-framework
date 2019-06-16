using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Textarea : IControl
    {
        private readonly Control _Parent;
        private readonly Site.Setting.Control.Textarea _Settings;

        public Textarea(Control parent, Site.Setting.Control.Textarea settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public DirectiveCollection Children => null;

        public void Parse()
        { }

        public void Render(string requesterUniqueID)
        {
            this.Parse();

            // Textarea needs to link ContentArguments of its parent.
            if (this._Parent.Parent != null)
                this._Parent.Arguments.Replace(this._Parent.Parent.Arguments);

            this._Parent.Bag.Add("content", this._Settings.Content, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render(requesterUniqueID);

            string renderedContent = this._Parent.Bag["content"].Result;

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            if (this._Settings.Security.Disabled.Set && 
                this._Settings.Security.Disabled.Type == SecurityDefinition.DisabledDefinition.Types.Dynamic)
                this._Parent.Deliver(RenderStatus.Rendered, this._Settings.Security.Disabled.Value);
            else
            {
                this._Parent.Deliver(
                    RenderStatus.Rendered,
                    string.Format(
                        "<textarea name=\"{0}\" id=\"{0}\"{1}>{2}</textarea>",
                        this._Parent.DirectiveID, this._Settings.Attributes, renderedContent
                    )
                );
            }
        }
    }
}