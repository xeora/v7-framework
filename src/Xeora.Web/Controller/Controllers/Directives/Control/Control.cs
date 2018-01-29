using System;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive.Control
{
    public abstract class Control : Directive, IControl
    {
        public Control(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments, ControlSettings settings) : 
            base(rawStartIndex, rawValue, DirectiveTypes.Control, contentArguments)
        {
            this.Settings = settings;

            this.Type = settings.Type;

            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
            this.BoundControlID = DirectiveHelper.CaptureBoundControlID(this.Value);
            this.Leveling = LevelingInfo.Create(this.Value);

            this.Security = settings.Security;
            this.Bind = settings.Bind;
            this.Attributes = settings.Attributes;
        }

        public string ControlID { get; private set; }
        public string BoundControlID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundControlID);
        public LevelingInfo Leveling { get; private set; }

        public ControlSettings Settings { get; private set; }

        public ControlTypes Type { get; private set; }
        public SecurityDefinition Security { get; private set; }
        public Basics.Execution.Bind Bind { get; protected set; }
        public AttributeDefinitionCollection Attributes { get; private set; }

        protected abstract void RenderControl(string requesterUniqueID);
        public abstract IControl Clone();

        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            if (!this.HasBound)
            {
                this.RenderControl(requesterUniqueID);

                return;
            }

            if (string.IsNullOrEmpty(requesterUniqueID))
                return;
            
            IController controller = null;
            this.Mother.Pool.GetInto(requesterUniqueID, out controller);

            if (controller != null &&
                controller is INamable &&
                string.Compare(((INamable)controller).ControlID, this.BoundControlID) == 0)
            {
                this.RenderControl(requesterUniqueID);

                return;
            }

            this.Mother.Scheduler.Register(this.BoundControlID, this.UniqueID);
        }

        protected string CleanJavascriptSignature(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string javascriptSignature = "javascript:";

            if (input.IndexOf(javascriptSignature) == 0)
                input = input.Substring(javascriptSignature.Length);

            if (input[input.Length - 1] == ';')
                input = input.Substring(0, input.Length - 1);

            return input;
        }
        protected string[] FixBlockIDs(IUpdateBlocks control)
        {
            if (control.BlockIDsToUpdate.Length == 0)
                return new string[] { this.UpdateBlockControlID };

            if (control.UpdateLocalBlock)
            {
                if (Array.IndexOf<string>(control.BlockIDsToUpdate, this.UpdateBlockControlID) == -1)
                {
                    string[] blockIDs = new string[control.BlockIDsToUpdate.Length + 1];
                    Array.Copy(control.BlockIDsToUpdate, blockIDs, control.BlockIDsToUpdate.Length);
                    blockIDs[blockIDs.Length - 1] = this.UpdateBlockControlID;

                    return blockIDs;
                }

                return control.BlockIDsToUpdate;
            }

            if (Array.IndexOf<string>(control.BlockIDsToUpdate, this.UpdateBlockControlID) > -1)
            {
                string[] blockIDs = new string[control.BlockIDsToUpdate.Length - 1];

                for (int c = 0, bC = 0; c < control.BlockIDsToUpdate.Length; c++)
                {
                    if (string.Compare(control.BlockIDsToUpdate[c], this.UpdateBlockControlID) != 0)
                    {
                        blockIDs[bC] = control.BlockIDsToUpdate[c];
                        bC++;
                    }
                }

                return blockIDs;
            }

            return control.BlockIDsToUpdate;
        }
    }
}