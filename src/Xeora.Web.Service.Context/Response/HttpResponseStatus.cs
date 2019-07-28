namespace Xeora.Web.Service.Context.Response
{
    public class HttpResponseStatus : Basics.Context.Response.IHttpResponseStatus
    {
        private short _Code;

        public HttpResponseStatus() =>
            this.Code = 200;

        public short Code 
        {
            get => this._Code;
            set
            {
                this._Code = value;
                this.Message = HttpResponseStatusCodes.StatusCodes.GetMessage(this._Code);
            }
        }
        public string Message { get; private set; }
    }
}
