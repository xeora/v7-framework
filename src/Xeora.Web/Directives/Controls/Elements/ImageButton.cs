using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class ImageButton : IControl
    {
        private readonly Control _Parent;
        private readonly Application.Controls.ImageButton _Settings;

        public ImageButton(Control parent, Application.Controls.ImageButton settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public DirectiveCollection Children => null;
        public bool LinkArguments => true;

        public void Parse()
        { }

        public void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this._Parent.Mother.UpdateBlockIdStack.Count > 0)
                this._Settings.Updates.Setup(this._Parent.Mother.UpdateBlockIdStack.Peek());

            this._Parent.Bag.Add("source", this._Settings.Source, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render(requesterUniqueId);

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
                    parameter => DirectiveHelper.RenderProperty(this._Parent, parameter.Query, this._Parent.Arguments, requesterUniqueId)
                );

                string xeoraCall;

                if (this._Parent.Mother.UpdateBlockIdStack.Count > 0)
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
                    this._Settings.Attributes["onclick"] = $"javascript:{xeoraCall};";
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
            {
                this._Parent.Deliver(RenderStatus.Rendered, this._Settings.Security.Disabled.Value);
                return;
            }
            
            this._Parent.Deliver(
                RenderStatus.Rendered,
                string.Format(
                    "<img name=\"{0}\" alt=\"\" id=\"{0}\"{1} />",
                    this._Parent.DirectiveId, this._Settings.Attributes
                )
            );
        }
    }
}
