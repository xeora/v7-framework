using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class ImageButton : IControl
    {
        private readonly Control _Parent;
        private readonly Site.Setting.Control.ImageButton _Settings;

        public ImageButton(Control parent, Site.Setting.Control.ImageButton settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public bool Searchable => false;

        public void Parse()
        { }

        public void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this._Parent.Mother.UpdateBlockIDStack.Count > 0)
                this._Settings.Updates.Setup(this._Parent.Mother.UpdateBlockIDStack.Peek());

            // ImageButton Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this._Parent.Parent != null)
                this._Parent.Arguments.Replace(this._Parent.Parent.Arguments);

            this._Parent.Bag.Add("source", this._Settings.Source, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render(requesterUniqueID);

            string renderedSource = this._Parent.Bag["source"].Result;

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            if (!string.IsNullOrEmpty(renderedSource))
                this._Settings.Attributes["src"] = renderedSource;
            else
                this._Settings.Attributes.Remove("src");

            // Define OnClick Server event for Button
            if (this._Settings.Bind != null)
            {
                // Render Bind Parameters
                this._Settings.Bind.Parameters.Prepare(
                    (parameter) => DirectiveHelper.RenderProperty(this._Parent, parameter.Query, this._Parent.Arguments, requesterUniqueID)
                );

                string xeoraCall;

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

                if (string.IsNullOrEmpty(this._Settings.Attributes["onclick"]))
                    this._Settings.Attributes["onclick"] = string.Format("javascript:{0};", xeoraCall);
                else
                {
                    this._Settings.Attributes["onclick"] = 
                        ControlHelper.CleanJavascriptSignature(this._Settings.Attributes["onclick"]);

                    this._Settings.Attributes["onclick"] =
                        string.Format(
                            "javascript:try{{{0};{1};}}catch(ex){{}};",
                            this._Settings.Attributes["onclick"], xeoraCall
                        );
                }
            }
            // !--

            if (this._Settings.Security.Disabled.Set && 
                this._Settings.Security.Disabled.Type == SecurityDefinition.DisabledDefinition.Types.Dynamic)
                this._Parent.Result = this._Settings.Security.Disabled.Value;
            else
            {
                this._Parent.Result =
                    string.Format(
                        "<img name=\"{0}\" alt=\"\" id=\"{0}\"{1} />",
                        this._Parent.DirectiveID, this._Settings.Attributes
                    );
            }
        }
    }
}
