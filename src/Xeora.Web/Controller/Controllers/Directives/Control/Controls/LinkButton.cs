using System;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive.Control
{
    public class LinkButton : Control, IHasText, IHasURL, IUpdateBlocks
    {
        public LinkButton(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        {
            this.Text = settings.Text;
            this.URL = settings.URL;
            this.UpdateLocalBlock = settings.UpdateLocalBlock;
            this.BlockIDsToUpdate = settings.BlockIDsToUpdate;
        }

        public string URL { get; private set; }
        public string Text { get; private set; }
        public bool UpdateLocalBlock { get; private set; }
        public string[] BlockIDsToUpdate { get; private set; }

        public override IControl Clone() =>
            new LinkButton(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            this.BlockIDsToUpdate = base.FixBlockIDs(this);

            // LinkButton Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            // Render Text Content
            this.Text = ControllerHelper.RenderSingleContent(this.Text, this, this.ContentArguments, requesterUniqueID);

            // href attribute is disabled always, use url attribute instand of...
            this.Attributes.Remove("href");
            this.Attributes.Remove("value");

            if (this.URL != null && this.URL.Trim().Length > 0)
            {
                if (this.URL.IndexOf("~/") == 0)
                {
                    this.URL = this.URL.Remove(0, 2);
                    this.URL = this.URL.Insert(0, Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);
                }
                else if (this.URL.IndexOf("¨/") == 0)
                {
                    this.URL = this.URL.Remove(0, 2);
                    this.URL = this.URL.Insert(0, Configurations.Xeora.Application.Main.VirtualRoot);
                }

                // Render URL Content
                this.URL = ControllerHelper.RenderSingleContent(this.URL, this, this.ContentArguments, requesterUniqueID);
                if (!string.IsNullOrEmpty(this.URL))
                    this.Attributes["href"] = this.URL;
            }

            if (string.IsNullOrWhiteSpace(this.URL))
                this.URL = "#_action0";

            if (this.Bind != null)
            {
                this.URL = "#_action1";

                if (this.Security.Disabled.IsSet && this.Security.Disabled.Type == SecurityInfo.DisabledClass.DisabledTypes.Dynamic)
                    this.Text = this.Security.Disabled.Value;
            }

            // Render Bind Parameters
            this.Bind = ControllerHelper.RenderBind(this.Bind, this, this.ContentArguments, requesterUniqueID);

            // Define OnClick Server event for Button
            if (this.Bind != null)
            {
                string xeoraCall;

                if (this.IsUpdateBlockController)
                {
                    xeoraCall = string.Format(
                        "__XeoraJS.update('{1}', '{0}')",
                        Manager.AssemblyCore.EncodeFunction(
                            Helpers.Context.HashCode,
                            this.Bind.ToString()
                        ),
                        string.Join(",", this.BlockIDsToUpdate)
                    );
                }
                else
                    xeoraCall = string.Format(
                            "__XeoraJS.post('{0}')",
                            Manager.AssemblyCore.EncodeFunction(
                                Helpers.Context.HashCode,
                                this.Bind.ToString()
                            )
                        );

                if (string.IsNullOrEmpty(this.Attributes["onclick"]))
                    this.Attributes["onclick"] = string.Format("javascript:{0};", xeoraCall);
                else
                {
                    this.Attributes["onclick"] = base.CleanJavascriptSignature(this.Attributes["onclick"]);

                    this.Attributes["onclick"] =
                        string.Format(
                            "javascript:try{{{0};{1};}}catch(ex){{}};",
                            this.Attributes["onclick"], xeoraCall
                        );
                }

            }
            // !--

            // Render Attributes
            for (int aC = 0; aC < this.Attributes.Count; aC++)
            {
                AttributeInfo item = this.Attributes[aC];

                this.Attributes[aC] =
                    new AttributeInfo(
                        item.Key,
                        ControllerHelper.RenderSingleContent(item.Value, this, this.ContentArguments, requesterUniqueID)
                    );
            }
            // !--

            if (this.Security.Disabled.IsSet && this.Security.Disabled.Type == SecurityInfo.DisabledClass.DisabledTypes.Dynamic)
                this.RenderedValue = this.Security.Disabled.Value;
            else
            {
                this.RenderedValue =
                    string.Format(
                        "<a name=\"{0}\" id=\"{0}\"{1}>{2}</a>",
                        this.ControlID, this.Attributes.ToString(), this.Text
                    );
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }
    }
}