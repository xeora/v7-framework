using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class RadioButton : IControl
    {
        private readonly Control _Parent;
        private readonly Site.Setting.Control.RadioButton _Settings;

        public RadioButton(Control parent, Site.Setting.Control.RadioButton settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public DirectiveCollection Children => null;
        public bool LinkArguments => true;

        public void Parse()
        { }

        public void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this._Parent.Mother.UpdateBlockIDStack.Count > 0)
                this._Settings.Updates.Setup(this._Parent.Mother.UpdateBlockIDStack.Peek());

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

            string itemIndex = 
                System.Convert.ToString(this._Parent.Arguments["_sys_ItemIndex"]);
            string radioButtonID = this._Parent.DirectiveID;

            if (!string.IsNullOrEmpty(itemIndex))
                radioButtonID = string.Format("{0}_{1}", this._Parent.DirectiveID, itemIndex);
            string radioButtonLabel = 
                string.Format("<label for=\"{0}\">{1}</label>", radioButtonID, this._Settings.Text);

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
                this._Parent.Deliver(RenderStatus.Rendered, this._Settings.Security.Disabled.Value);
            else
            {
                this._Parent.Deliver(
                    RenderStatus.Rendered,
                    string.Format(
                        "<input type=\"radio\" name=\"{0}\" id=\"{1}\"{2}>{3}",
                        this._Parent.DirectiveID,
                        radioButtonID,
                        this._Settings.Attributes.ToString(),
                        radioButtonLabel
                    )
                );
            }
        }
    }
}