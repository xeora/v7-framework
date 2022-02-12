using System;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;
using Attribute = Xeora.Web.Basics.Domain.Control.Attribute;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Button : IControl
    {
        private readonly Control _Parent;
        private readonly Application.Controls.Button _Settings;

        public Button(Control parent, Application.Controls.Button settings)
        {
            this._Parent = parent;
            this._Settings = settings;
        }

        public bool LinkArguments => true;

        public void Parse()
        {
            if (this._Parent.UpdateBlockIds.Count > 0)
                this._Settings.Updates.Setup(string.Join(">", this._Parent.UpdateBlockIds.ToArray()));

            this._Parent.Bag.Add("text", this._Settings.Text, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);
            this._Parent.Bag.Render();
            
            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            string renderedText = 
                this._Parent.Bag["text"].Result;
            if (!string.IsNullOrEmpty(renderedText))
                this._Settings.Attributes["value"] = renderedText;
            else
                this._Settings.Attributes.Remove("value");

            // Define OnClick Server event for Button
            if (this._Settings.Bind != null)
            {
                // Render Bind Parameters
                this._Settings.Bind.Parameters.Prepare(
                    parameter =>
                    {
                        Tuple<bool, object> result =
                            Property.Render(this._Parent, parameter.Query);
                        return result.Item2;
                    });

                ICryptography cryptography =
                    CryptographyProvider.Current.Get(Helpers.Context.Session.SessionId);
                string xeoraCall;

                if (this._Parent.UpdateBlockIds.Count > 0)
                {
                    xeoraCall = string.Format(
                        "__XeoraJS.update('{1}', '{0}')",
                        cryptography.Encrypt(
                            this._Settings.Bind.ToString()
                        ),
                        string.Join(",", this._Settings.Updates.Blocks)
                    );
                }
                else
                    xeoraCall = string.Format(
                            "__XeoraJS.post('{0}')",
                            cryptography.Encrypt(
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
                if (!string.IsNullOrEmpty(this._Settings.Security.Disabled.Value))
                    this._Parent.Children.Add(new Static(this._Settings.Security.Disabled.Value));
                return;
            }

            this._Parent.Children.Add(
                new Static(
            string.Format(
                           "<input type=\"button\" name=\"{0}\" id=\"{0}\"{1} />",
                           this._Parent.DirectiveId, this._Settings.Attributes
                       )
                )
            );
        }
    }
}
