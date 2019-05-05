using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class ConditionalStatement : IControl
    {
        private int _SelectedContent = -1;

        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Site.Setting.Control.ConditionalStatement _Settings;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public ConditionalStatement(Control parent, ContentDescription contents, string[] parameters, Site.Setting.Control.ConditionalStatement settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public DirectiveCollection Children => this._Children;

        public void Parse()
        {
            if (this._SelectedContent == -1)
                return;

            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this._Parent.Mother, this._Parent);

            this._Parent.Mother.RequestParsing(
                this._Contents.Parts[this._SelectedContent], ref this._Children, this._Parent.Arguments);
        }

        public void Render(string requesterUniqueID)
        {
            // ConditionalStatment does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this._Parent.Parent != null)
                this._Parent.Arguments.Replace(this._Parent.Parent.Arguments);

            // Call Related Function and Exam It
            IDirective leveledDirective = this._Parent;
            int level = this._Parent.Leveling.Level;

            do
            {
                if (level == 0)
                    break;

                leveledDirective = leveledDirective.Parent;

                if (leveledDirective is Renderless)
                    leveledDirective = leveledDirective.Parent;

                level -= 1;
            } while (leveledDirective != null);

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this._Settings.Bind.Parameters.Prepare(
                (parameter) =>
                {
                    string query = parameter.Query;

                    int paramIndex = 
                        DirectiveHelper.CaptureParameterPointer(query);

                    if (paramIndex > -1)
                    { 
                        if (paramIndex >= this._Parameters.Length)
                            throw new Exception.FormatIndexOutOfRangeException();

                        query = this._Parameters[paramIndex];
                    }

                    return DirectiveHelper.RenderProperty(leveledDirective.Parent, query, leveledDirective.Parent.Arguments, requesterUniqueID);
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.Conditional> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.Conditional>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);
            // ----

            if (invokeResult.Result == null)
                return;

            switch (invokeResult.Result.Result)
            {
                case Basics.ControlResult.Conditional.Conditions.True:
                    if (string.IsNullOrEmpty(this._Contents.Parts[0]))
                        break;

                    if (!this._Parent.Leveling.ExecutionOnly)
                    {
                        this._Parent.Arguments.Replace(leveledDirective.Arguments);
                        this._Children.OverrideParent(leveledDirective);
                    }

                    this._SelectedContent = 0;
                    this.Parse();

                    break;
                case Basics.ControlResult.Conditional.Conditions.False:
                    if (this._Contents.Parts.Count < 2)
                        break;

                    if (string.IsNullOrEmpty(this._Contents.Parts[1]))
                        break;

                    if (!this._Parent.Leveling.ExecutionOnly)
                    {
                        this._Parent.Arguments.Replace(leveledDirective.Arguments);
                        this._Children.OverrideParent(leveledDirective);
                    }

                    this._SelectedContent = 1;
                    this.Parse();

                    break;
                case Basics.ControlResult.Conditional.Conditions.Unknown:
                    // Reserved For Future Uses

                    break;
            }

            if (this._SelectedContent > -1)
            {
                this._Children.Render(requesterUniqueID);
                this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
            }
        }
    }
}