using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Textarea : IControl
    {
        private readonly Control _Parent;
        private readonly Application.Controls.Textarea _Settings;

        public Textarea(Control parent, Application.Controls.Textarea settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public bool LinkArguments => true;

        public void Parse()
        {
            this._Parent.Bag.Add("content", this._Settings.Content, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render();

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            string renderedContent = 
                this._Parent.Bag["content"].Result;
            
            if (this._Settings.Security.Disabled.Set &&
                this._Settings.Security.Disabled.Type == SecurityDefinition.DisabledDefinition.Types.Dynamic)
            {
                this._Parent.Children.Add(
                    new Static(this._Settings.Security.Disabled.Value));
                return;
            }

            this._Parent.Children.Add(
                new Static(
                    string.Format(
                    "<textarea name=\"{0}\" id=\"{0}\"{1}>{2}</textarea>",
                    this._Parent.DirectiveId, this._Settings.Attributes, renderedContent
                    )
                )
            );
        }
    }
}