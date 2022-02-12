using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class VariableBlock : IControl
    {
        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Application.Controls.VariableBlock _Settings;

        public VariableBlock(Control parent, ContentDescription contents, string[] parameters, Application.Controls.VariableBlock settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public bool LinkArguments => false;

        public void Parse()
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
                        return Property.Render(this._Parent, query).Item2;
                    
                    if (paramIndex >= this._Parameters.Length)
                        throw new Exceptions.FormatIndexOutOfRangeException("VariableBlock");

                    return Property.Render(this._Parent, this._Parameters[paramIndex]).Item2;
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.VariableBlock> invokeResult =
                Manager.Executer.InvokeBind<Basics.ControlResult.VariableBlock>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception);
            // ----

            if (invokeResult.Result == null)
                return;

            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                {
                    this._Parent.Children.Clear();
                    if (!string.IsNullOrEmpty(invokeResult.Result.Message.Content))
                        this._Parent.Children.Add(new Static(invokeResult.Result.Message.Content));
                }
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);
                    
                    this._Parent.Mother.RequestParsing(this._Contents.MessageTemplate, this._Parent.Children, this._Parent.Arguments);
                }

                return;
            }

            if (invokeResult.Result != null)
            {
                foreach (string key in invokeResult.Result.Keys)
                    this._Parent.Arguments.AppendKeyWithValue(key, invokeResult.Result[key]);
            }
            
            this._Parent.Mother.RequestParsing(this._Contents.Parts[0], this._Parent.Children, this._Parent.Arguments);
        }
    }
}
