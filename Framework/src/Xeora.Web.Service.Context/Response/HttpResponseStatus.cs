namespace Xeora.Web.Service.Context
{
    public class HttpResponseStatus : Basics.Context.IHttpResponseStatus
    {
        public HttpResponseStatus()
        {
            this.Code = 200;
            this.Message = "OK";
        }

        public short Code { get; set; }
        public string Message { get; set; }
    }
}
