using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class VariableBlock : IControl
    {
        private int _SelectedContent = -1;

        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Site.Setting.Control.VariableBlock _Settings;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public VariableBlock(Control parent, ContentDescription contents, string[] parameters, Site.Setting.Control.VariableBlock settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public bool Searchable => true;

        public void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this._Parent.Mother, this._Parent);

            this._Parent.Mother.RequestParsing(
                this._SelectedContent == -1 ? this._Contents.MessageTemplate : this._Contents.Parts[0], ref this._Children, this._Parent.Arguments);
        }

        public void Render(string requesterUniqueID)
        {
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

            Basics.Execution.InvokeResult<Basics.ControlResult.VariableBlock> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.VariableBlock>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);
            // ----

            if (invokeResult.Result == null)
                return;

            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Deliver(RenderStatus.Rendered, invokeResult.Result.Message.Content);
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                    this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
                }

                return;
            }

            if (!this._Parent.Leveling.ExecutionOnly)
            {
                this._Parent.Arguments.Replace(leveledDirective.Arguments);
                this._Children.OverrideParent(leveledDirective);
            }

            if (invokeResult.Result != null)
            {
                foreach (string key in invokeResult.Result.Keys)
                    this._Parent.Arguments.AppendKeyWithValue(key, invokeResult.Result[key]);
            }

            this._SelectedContent = 0;
            this.Parse();
            this._Children.Render(requesterUniqueID);
            this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
        }
    }
}
