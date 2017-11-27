namespace Xeora.Web.Basics.Attribute
{
    public class HttpMethodAttribute : System.Attribute
    {
        public enum Methods
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public HttpMethodAttribute() : this(Methods.GET, string.Empty)
        { }

        public HttpMethodAttribute(Methods method) : this(method, string.Empty)
        { }

        public HttpMethodAttribute(Methods method, string bindProcedureName)
        {
            this.Method = method;

            if (bindProcedureName == null)
                bindProcedureName = string.Empty;
            this.BindProcedureName = bindProcedureName;
        }

        public Methods Method { get; private set; }
        public string BindProcedureName { get; private set; }
    }
}