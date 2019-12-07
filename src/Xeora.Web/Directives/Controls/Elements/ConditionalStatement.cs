using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class ConditionalStatement : IControl
    {
        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Application.Controls.ConditionalStatement _Settings;

        public ConditionalStatement(Control parent, ContentDescription contents, string[] parameters, Application.Controls.ConditionalStatement settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }
        
        public bool LinkArguments => true;

        public void Parse()
        {
            if (this._Settings.Bind == null)
                throw new System.ArgumentNullException(nameof(this._Settings.Bind));

            this._Settings.Bind.Parameters.Prepare(
                parameter =>
                {
                    string query = parameter.Query;
                    int paramIndex =
                        DirectiveHelper.CaptureParameterPointer(query);

                    if (paramIndex < 0)
                        return Property.Render(this._Parent, query).Item2;
                    
                    if (paramIndex >= this._Parameters.Length)
                        throw new Exceptions.FormatIndexOutOfRangeException("ConditionalStatement");

                    return Property.Render(this._Parent, this._Parameters[paramIndex]).Item2;
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.Conditional> invokeResult =
                Manager.Executer.InvokeBind<Basics.ControlResult.Conditional>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);
            // ----

            if (invokeResult.Result == null)
                return;

            switch (invokeResult.Result.Result)
            {
                case Basics.ControlResult.Conditional.Conditions.True:
                    if (string.IsNullOrEmpty(this._Contents.Parts[0]))
                        break;

                    this._Parent.Mother.RequestParsing(
                        this._Contents.Parts[0], this._Parent.Children, this._Parent.Arguments);

                    break;
                case Basics.ControlResult.Conditional.Conditions.False:
                    if (this._Contents.Parts.Count < 2)
                        break;

                    if (string.IsNullOrEmpty(this._Contents.Parts[1]))
                        break;

                    this._Parent.Mother.RequestParsing(
                        this._Contents.Parts[1], this._Parent.Children, this._Parent.Arguments);

                    break;
                case Basics.ControlResult.Conditional.Conditions.Unknown:
                    // Reserved For Future Uses
                    return;
            }
        }
    }
}