using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class VariableBlock : IControl, IHasChildren
    {
        private int _SelectedContent = -1;

        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Application.Controls.VariableBlock _Settings;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public VariableBlock(Control parent, ContentDescription contents, string[] parameters, Application.Controls.VariableBlock settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public DirectiveCollection Children => this._Children;
        public bool LinkArguments => false;

        public void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this._Parent.Mother, this._Parent);

            this._Parent.Mother.RequestParsing(
                this._SelectedContent == -1 ? this._Contents.MessageTemplate : this._Contents.Parts[0], ref this._Children, this._Parent.Arguments);
        }

        public void Render(string requesterUniqueId)
        {
            if (this._Settings.Bind == null)
                throw new System.ArgumentNullException(nameof(this._Settings.Bind));

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this._Settings.Bind.Parameters.Prepare(
                parameter =>
                {
                    string query = parameter.Query;
                    int paramIndex =
                        DirectiveHelper.CaptureParameterPointer(query);

                    if (paramIndex < 0)
                        return DirectiveHelper.RenderProperty(
                            this._Parent.Parent, query, 
                            this._Parent.Parent.Arguments,
                            requesterUniqueId
                        );
                    
                    if (paramIndex >= this._Parameters.Length)
                        throw new Exceptions.FormatIndexOutOfRangeException();

                    return DirectiveHelper.RenderProperty(
                            this._Parent.Parent, this._Parameters[paramIndex], 
                            this._Parent.Parent.Arguments, 
                            requesterUniqueId
                        );
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.VariableBlock> invokeResult =
                Manager.Executer.InvokeBind<Basics.ControlResult.VariableBlock>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);
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
                    this._Children.Render(requesterUniqueId);
                    this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
                }

                return;
            }

            if (invokeResult.Result != null)
            {
                foreach (string key in invokeResult.Result.Keys)
                    this._Parent.Arguments.AppendKeyWithValue(key, invokeResult.Result[key]);
            }

            this._SelectedContent = 0;
            this.Parse();
            this._Children.Render(requesterUniqueId);
            this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
        }
    }
}
