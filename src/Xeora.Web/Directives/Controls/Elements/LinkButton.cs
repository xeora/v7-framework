using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class LinkButton : IControl
    {
        private readonly Control _Parent;
        private readonly Application.Domain.Controls.LinkButton _Settings;

        public LinkButton(Control parent, Application.Domain.Controls.LinkButton settings)
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

            this._Parent.Bag.Add("text", this._Settings.Text, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);

            // href attribute is disabled always, use url attribute instead of...
            this._Settings.Attributes.Remove("href");
            this._Settings.Attributes.Remove("value");

            string parsedUrl = this._Settings.Url;

            if (!string.IsNullOrEmpty(parsedUrl))
            {
                if (parsedUrl.IndexOf("~/", System.StringComparison.InvariantCulture) == 0)
                {
                    parsedUrl = parsedUrl.Remove(0, 2);
                    parsedUrl = parsedUrl.Insert(0, Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);
                }
                else if (parsedUrl.IndexOf("¨/", System.StringComparison.InvariantCulture) == 0)
                {
                    parsedUrl = parsedUrl.Remove(0, 2);
                    parsedUrl = parsedUrl.Insert(0, Configurations.Xeora.Application.Main.VirtualRoot);
                }

                this._Parent.Bag.Add("url", parsedUrl, this._Parent.Arguments);
            }

            this._Parent.Bag.Render(requesterUniqueId);

            string renderedText = this._Parent.Bag["text"].Result;
            string renderedUrl = string.Empty;
            if (this._Parent.Bag.ContainsKey("url"))
                renderedUrl = this._Parent.Bag["url"].Result;

            if (!string.IsNullOrEmpty(renderedUrl))
                this._Settings.Attributes["href"] = renderedUrl;
            else
                this._Settings.Attributes["href"] = "#_action0";

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            // Define OnClick Server event for Button
            if (this._Settings.Bind != null)
            {
                this._Settings.Attributes["href"] = "#_action1";

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
                    "<a name=\"{0}\" id=\"{0}\"{1}>{2}</a>",
                    this._Parent.DirectiveId, this._Settings.Attributes, renderedText
                )
            );
        }
    }
}