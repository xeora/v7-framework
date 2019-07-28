using System.Collections.Generic;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Directives.Controls;
using Xeora.Web.Directives.Controls.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Control : Directive, INameable, IBoundable, ILevelable, IHasChildren
    {
        private readonly string _RawValue;
        private IControl _Control;

        public Control(string rawValue, ArgumentCollection arguments) : 
            base(DirectiveTypes.Control, arguments)
        {
            this._RawValue = rawValue;
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this.BoundDirectiveId = DirectiveHelper.CaptureBoundDirectiveId(rawValue);
            this.Leveling = LevelingInfo.Create(rawValue);
        }

        public void Load()
        {
            this.Mother.RequestInstance(out IDomain instance);
            this.Mother.RequestControlResolve(this.DirectiveId, ref instance, out IBase control);

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
                            (Application.Controls.ConditionalStatement)control
                        );

                    break;
                case ControlTypes.DataList:
                    this._Control =
                        new DataList(
                            this,
                            new ContentDescription(this._RawValue),
                            DirectiveHelper.CaptureControlParameters(this._RawValue),
                            (Application.Controls.DataList)control
                        );

                    break;
                case ControlTypes.VariableBlock:
                    this._Control =
                        new VariableBlock(
                            this,
                            new ContentDescription(this._RawValue),
                            DirectiveHelper.CaptureControlParameters(this._RawValue),
                            (Application.Controls.VariableBlock)control
                        );

                    break;
                case ControlTypes.Button:
                    this._Control = new Button(this, (Application.Controls.Button)control);

                    break;
                case ControlTypes.Checkbox:
                    this._Control = new Checkbox(this, (Application.Controls.Checkbox)control);

                    break;
                case ControlTypes.ImageButton:
                    this._Control = new ImageButton(this, (Application.Controls.ImageButton)control);

                    break;
                case ControlTypes.LinkButton:
                    this._Control = new LinkButton(this, (Application.Controls.LinkButton)control);

                    break;
                case ControlTypes.Password:
                    this._Control = new Password(this, (Application.Controls.Password)control);

                    break;
                case ControlTypes.RadioButton:
                    this._Control = new RadioButton(this, (Application.Controls.RadioButton)control);

                    break;
                case ControlTypes.Textarea:
                    this._Control = new Textarea(this, (Application.Controls.Textarea)control);

                    break;
                case ControlTypes.Textbox:
                    this._Control = new Textbox(this, (Application.Controls.Textbox)control);

                    break;
            }

            this.Type = control.Type;
        }

        public string DirectiveId { get; }
        public string BoundDirectiveId { get; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveId);
        public LevelingInfo Leveling { get; }
        public new ControlTypes Type { get; private set; }

        public override bool Searchable
        {
            get
            {
                switch (this.Type)
                {
                    case ControlTypes.ConditionalStatement:
                    case ControlTypes.VariableBlock:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Control.Children;

        public RenderBag Bag { get; private set; }

        public override void Parse() =>
            this._Control.Parse();

        public override void Render(string requesterUniqueId)
        {
            if (this.HasBound)
            {
                if (string.IsNullOrEmpty(requesterUniqueId))
                    return;

                this.Mother.Pool.GetByDirectiveId(this.BoundDirectiveId, out IEnumerable<IDirective> directives);

                if (directives == null) return;

                foreach (IDirective directive in directives)
                {
                    if (!(directive is INameable)) return;

                    string directiveId = ((INameable)directive).DirectiveId;
                    if (string.CompareOrdinal(directiveId, this.BoundDirectiveId) != 0) return;

                    if (directive.Status == RenderStatus.Rendered) continue;
                    
                    directive.Scheduler.Register(this.UniqueId);
                    return;
                }
            }

            int level = this.Leveling.Level;
            IDirective leveledParentDirective = this.Parent;

            while (level > 0)
            {
                leveledParentDirective = leveledParentDirective.Parent;
                level--;

                if (leveledParentDirective != null) continue;
                
                leveledParentDirective = this.Parent;
                break;
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

            this._Control.Render(requesterUniqueId);

            this.Scheduler.Fire();
        }
    }
}