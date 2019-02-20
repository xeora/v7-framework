using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class LinkButton : IControl
    {
        private readonly Control _Parent;
        private readonly Site.Setting.Control.LinkButton _Settings;

        public LinkButton(Control parent, Site.Setting.Control.LinkButton settings)
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

            // LinkButton Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this._Parent.Parent != null)
                this._Parent.Arguments.Replace(this._Parent.Parent.Arguments);

            this._Parent.Bag.Add("text", this._Settings.Text, this._Parent.Arguments);
            foreach (Attribute item in this._Settings.Attributes)
                this._Parent.Bag.Add(item.Key, item.Value, this._Parent.Arguments);

            // href attribute is disabled always, use url attribute instand of...
            this._Settings.Attributes.Remove("href");
            this._Settings.Attributes.Remove("value");

            string parsedURL = this._Settings.URL;

            if (!string.IsNullOrEmpty(parsedURL))
            {
                if (parsedURL.IndexOf("~/") == 0)
                {
                    parsedURL = parsedURL.Remove(0, 2);
                    parsedURL = parsedURL.Insert(0, Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);
                }
                else if (parsedURL.IndexOf("¨/") == 0)
                {
                    parsedURL = parsedURL.Remove(0, 2);
                    parsedURL = parsedURL.Insert(0, Configurations.Xeora.Application.Main.VirtualRoot);
                }

                this._Parent.Bag.Add("url", parsedURL, this._Parent.Arguments);

            }

            this._Parent.Bag.Render(requesterUniqueID);

            string renderedText = this._Parent.Bag["text"].Result;
            string renderedURL = string.Empty;
            if (this._Parent.Bag.ContainsKey("url"))
                renderedURL = this._Parent.Bag["url"].Result;

            if (!string.IsNullOrEmpty(renderedURL))
                this._Settings.Attributes["href"] = renderedURL;
            else
                renderedURL = "#_action0";

            for (int aC = 0; aC < this._Settings.Attributes.Count; aC++)
            {
                Attribute item = this._Settings.Attributes[aC];
                this._Settings.Attributes[aC] =
                    new Attribute(item.Key, this._Parent.Bag[item.Key].Result);
            }

            // Define OnClick Server event for Button
            if (this._Settings.Bind != null)
            {
                renderedURL = "#_action1";

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
                        "<a name=\"{0}\" id=\"{0}\"{1}>{2}</a>",
                        this._Parent.DirectiveID, this._Settings.Attributes, renderedText
                    );
            }
        }
    }
}