using System;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive.Control
{
    public class RadioButton : Control, IHasText, IUpdateBlocks
    {
        public RadioButton(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        {
            this.Text = settings.Text;
            this.UpdateLocalBlock = settings.UpdateLocalBlock;
            this.BlockIDsToUpdate = settings.BlockIDsToUpdate;
        }

        public string Text { get; private set; }
        public bool UpdateLocalBlock { get; private set; }
        public string[] BlockIDsToUpdate { get; private set; }

        public override IControl Clone() =>
            new RadioButton(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);

        protected override void RenderControl(string requesterUniqueID)
        {
            this.BlockIDsToUpdate = base.FixBlockIDs(this);

            // RadioButton Control does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            // Render Text Content
            this.Text = ControllerHelper.RenderSingleContent(this.Text, this, this.ContentArguments, requesterUniqueID);

            string itemIndex = (string)this.ContentArguments["_sys_ItemIndex"];
            string radioButtonID = this.ControlID;

            if (itemIndex != null)
                radioButtonID = string.Format("{0}_{1}", this.ControlID, itemIndex);
            string radioButtonLabel = 
                string.Format("<label for=\"{0}\">{1}</label>", radioButtonID, this.Text);

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
                        "<input type=\"radio\" name=\"{0}\" id=\"{1}\"{2}>{3}",
                        this.ControlID,
                        radioButtonID,
                        this.Attributes.ToString(),
                        radioButtonLabel
                    );
            }

            this.Mother.Scheduler.Fire(this.ControlID);
        }
    }
}