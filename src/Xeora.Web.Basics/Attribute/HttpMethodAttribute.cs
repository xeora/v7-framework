namespace Xeora.Web.Basics.Attribute
{
    public class HttpMethodAttribute : System.Attribute
    {
        public HttpMethodAttribute() : this(Context.HttpMethod.GET, string.Empty)
        { }

        public HttpMethodAttribute(Context.HttpMethod method) : this(method, string.Empty)
        { }

        public HttpMethodAttribute(Context.HttpMethod method, string bindProcedureName)
        {
            this.Method = method;

            if (bindProcedureName == null)
                bindProcedureName = string.Empty;
            this.BindProcedureName = bindProcedureName;
        }

        public Context.HttpMethod Method { get; private set; }
        public string BindProcedureName { get; private set; }
    }
}