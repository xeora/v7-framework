using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Textbox : IControl
    {
        private readonly Control _Parent;
        private readonly Site.Setting.Control.Textbox _Settings;

        public Textbox(Control parent, Site.Setting.Control.Textbox settings)
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

            if (this._Parent.Mother.UpdateBlockIDStack.Count > 0)
                this._Settings.Updates.Setup(this._Parent.Mother.UpdateBlockIDStack.Peek());

            // Textbox Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this._Parent.Parent != null)
                this._Parent.Arguments.Replace(this._Parent.Parent.Arguments);

            this._Parent.Bag.Add("text", this._Settings.Text, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render(requesterUniqueID);

            string renderedText = this._Parent.Bag["text"].Result;

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            if (!string.IsNullOrEmpty(renderedText))
                this._Settings.Attributes["value"] = renderedText;
            else
                this._Settings.Attributes.Remove("value");

            // Define onKeyDown Server event for Button
            string xeoraCall = string.Empty;

            if (this._Settings.Bind != null)
            {
                // Render Bind Parameters
                this._Settings.Bind.Parameters.Prepare(
                    (parameter) => DirectiveHelper.RenderProperty(this._Parent, parameter.Query, this._Parent.Arguments, requesterUniqueID)
                );

                if (this._Parent.Mother.UpdateBlockIDStack.Count > 0)
                {
                    xeoraCall = string.Format(
                        "__XeoraJS.update('{1}', '{0}')",
                        Manager.AssemblyCore.EncodeFunction(
                            Helpers.Context.HashCode,
                            this._Settings.Bind.ToString()
                        ),
                        string.Join(",", this._Settings.Updates.Blocks)
                    );
                }
                else
                    xeoraCall = string.Format(
                            "__XeoraJS.post('{0}')",
                            Manager.AssemblyCore.EncodeFunction(
                                Helpers.Context.HashCode,
                                this._Settings.Bind.ToString()
                            )
                        );
            }

            string keydownEvent = 
                ControlHelper.CleanJavascriptSignature(this._Settings.Attributes["onkeydown"]);

            if (!string.IsNullOrEmpty(xeoraCall))
                keydownEvent = string.Format("{0}{1};", keydownEvent, xeoraCall);

            if (!string.IsNullOrEmpty(this._Settings.DefaultButtonID))
            {
                keydownEvent =
                    string.Format(
                        "{0}if(event.keyCode==13){{document.getElementById('{1}').click();}};",
                        keydownEvent, this._Settings.DefaultButtonID
                    );
            }

            if (!string.IsNullOrEmpty(keydownEvent))
                keydownEvent = string.Format("javascript:try{{{0}}}catch(ex){{}};", keydownEvent);

            this._Settings.Attributes["onkeydown"] = keydownEvent;
            // !--

            if (this._Settings.Security.Disabled.Set && 
                this._Settings.Security.Disabled.Type == SecurityDefinition.DisabledDefinition.Types.Dynamic)
                this._Parent.Deliver(RenderStatus.Rendered, this._Settings.Security.Disabled.Value);
            else
            {
                this._Parent.Deliver(
                    RenderStatus.Rendered,
                    string.Format(
                        "<input type=\"text\" name=\"{0}\" id=\"{0}\"{1}>",
                        this._Parent.DirectiveID, this._Settings.Attributes
                    )
                );
            }
        }
    }
}
