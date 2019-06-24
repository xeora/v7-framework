using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Directives.Controls;
using Xeora.Web.Directives.Controls.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Control : Directive, INamable, IBoundable, ILevelable, IHasChildren
    {
        private readonly string _RawValue;
        private IControl _Control;

        public Control(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.Control, arguments)
        {
            this._RawValue = rawValue;
            this.DirectiveID = DirectiveHelper.CaptureDirectiveID(rawValue);
            this.BoundDirectiveID = DirectiveHelper.CaptureBoundDirectiveID(rawValue);
            this.Leveling = LevelingInfo.Create(rawValue);
        }

        public void Load()
        {
            IDomain instance = null;
            this.Mother.RequestInstance(ref instance);
            this.Mother.RequestControlResolve(this.DirectiveID, ref instance, out IBase control);

            switch (control.Type)
            {
                case ControlTypes.Unknown:
                    this._Control = new Unknown(this);

                    break;
                case ControlTypes.ConditionalStatement:
                    this._Control =
                        new ConditionalStatement(
                            this,
                            new ContentDescription(this._RawValue),
                            DirectiveHelper.CaptureControlParameters(this._RawValue),
                            (Site.Setting.Control.ConditionalStatement)control
                        );

                    break;
                case ControlTypes.DataList:
                    this._Control =
                        new DataList(
                            this,
                            new ContentDescription(this._RawValue),
                            DirectiveHelper.CaptureControlParameters(this._RawValue),
                            (Site.Setting.Control.DataList)control
                        );

                    break;
                case ControlTypes.VariableBlock:
                    this._Control =
                        new VariableBlock(
                            this,
                            new ContentDescription(this._RawValue),
                            DirectiveHelper.CaptureControlParameters(this._RawValue),
                            (Site.Setting.Control.VariableBlock)control
                        );

                    break;
                case ControlTypes.Button:
                    this._Control = new Button(this, (Site.Setting.Control.Button)control);

                    break;
                case ControlTypes.Checkbox:
                    this._Control = new Checkbox(this, (Site.Setting.Control.Checkbox)control);

                    break;
                case ControlTypes.ImageButton:
                    this._Control = new ImageButton(this, (Site.Setting.Control.ImageButton)control);

                    break;
                case ControlTypes.LinkButton:
                    this._Control = new LinkButton(this, (Site.Setting.Control.LinkButton)control);

                    break;
                case ControlTypes.Password:
                    this._Control = new Password(this, (Site.Setting.Control.Password)control);

                    break;
                case ControlTypes.RadioButton:
                    this._Control = new RadioButton(this, (Site.Setting.Control.RadioButton)control);

                    break;
                case ControlTypes.Textarea:
                    this._Control = new Textarea(this, (Site.Setting.Control.Textarea)control);

                    break;
                case ControlTypes.Textbox:
                    this._Control = new Textbox(this, (Site.Setting.Control.Textbox)control);

                    break;
            }

            this.Type = control.Type;
        }

        public string DirectiveID { get; private set; }
        public string BoundDirectiveID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveID);
        public LevelingInfo Leveling { get; private set; }
        public new ControlTypes Type { get; private set; }

        public override bool Searchable => this._Control.Children != null;
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Control.Children;

        public RenderBag Bag { get; private set; }

        public override void Parse() =>
            this._Control.Parse();

        public override void Render(string requesterUniqueID)
        {
            if (this.HasBound)
            {
                if (string.IsNullOrEmpty(requesterUniqueID))
                    return;

                this.Mother.Pool.GetByDirectiveID(this.BoundDirectiveID, out IDirective[] directives);

                if (directives == null) return;

                foreach (IDirective directive in directives)
                {
                    if (!(directive is INamable)) return;

                    string directiveID = ((INamable)directive).DirectiveID;
                    if (string.Compare(directiveID, this.BoundDirectiveID) != 0) return;

                    if (directive.Status != RenderStatus.Rendered)
                    {
                        directive.Scheduler.Register(this.UniqueID);
                        return;
                    }
                }
            }

            int level = this.Leveling.Level;
            IDirective leveledParentDirective = this.Parent;

            while (level > 0)
            {
                leveledParentDirective = leveledParentDirective.Parent;
                level--;

                if (leveledParentDirective == null)
                {
                    leveledParentDirective = this.Parent;
                    break;
                }
            }

            if (!this.Leveling.ExecutionOnly)
                this.Arguments.Replace(leveledParentDirective.Arguments);

            this.Parent = leveledParentDirective;
            this.Bag = new RenderBag(leveledParentDirective);

            if (this._Control.LinkArguments && this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this._Control.Render(requesterUniqueID);

            this.Scheduler.Fire();
        }
    }
}