namespace Xeora.Web.Basics.Attribute
{
    public class HttpMethodAttribute : System.Attribute
    {
        public HttpMethodAttribute() : this(Context.Request.HttpMethod.GET, string.Empty)
        { }

        public HttpMethodAttribute(Context.Request.HttpMethod method) : this(method, string.Empty)
        { }

        public HttpMethodAttribute(Context.Request.HttpMethod method, string bindProcedureName)
        {
            this.Method = method;

            if (bindProcedureName == null)
                bindProcedureName = string.Empty;
            this.BindProcedureName = bindProcedureName;
        }

        public Context.Request.HttpMethod Method { get; }
        public string BindProcedureName { get; }
    }
}