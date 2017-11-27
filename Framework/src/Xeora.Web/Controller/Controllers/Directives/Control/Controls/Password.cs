using System;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive.Control
{
    public class Password : Control, IHasText, IHasDefaultButton, IUpdateBlocks
    {
        public Password(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        {
            this.DefaultButtonID = settings.DefaultButtonID;
            this.Text = settings.Text;
            this.UpdateLocalBlock = settings.UpdateLocalBlock;
            this.BlockIDsToUpdate = settings.BlockIDsToUpdate;
        }

        public string DefaultButtonID { get; private set; }
        public string Text { get; private set; }
        public bool UpdateLocalBlock { get; private set; }
        public string[] BlockIDsToUpdate { get; private set; }

        public override IControl Clone() =>
            new Password(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            this.BlockIDsToUpdate = base.FixBlockIDs(this);

            // Password Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            // Render Text Content
            this.Text = ControllerHelper.RenderSingleContent(this.Text, this, this.ContentArguments, requesterUniqueID);
            if (!string.IsNullOrEmpty(this.Text))
                this.Attributes["value"] = this.Text;
            else
                this.Attributes.Remove("value");

            // Render Bind Parameters
            this.Bind = ControllerHelper.RenderBindInfo(this.Bind, this, this.ContentArguments, requesterUniqueID);

            // Define onKeyDown Server event for Button
            string xeoraCall = string.Empty;

            if (this.Bind != null)
            {
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
            }

            string keydownEvent = this.Attributes["onkeydown"];

            keydownEvent = base.CleanJavascriptSignature(keydownEvent);

            if (!string.IsNullOrEmpty(xeoraCall))
                keydownEvent = string.Format("{0}{1};", keydownEvent, xeoraCall);

            if (!string.IsNullOrEmpty(this.DefaultButtonID))
            {
                keydownEvent =
                    string.Format(
                        "{0}if(event.keyCode==13){{document.getElementById('{1}').click();}};",
                        keydownEvent, this.DefaultButtonID
                    );
            }

            if (!string.IsNullOrEmpty(keydownEvent))
                keydownEvent = string.Format("javascript:try{{{0}}}catch(ex){{}};", keydownEvent);

            this.Attributes["onkeydown"] = keydownEvent;
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
                        "<input type=\"password\" name=\"{0}\" id=\"{0}\"{1}>",
                        this.ControlID, this.Attributes.ToString()
                    );
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }
    }
}