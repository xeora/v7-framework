using System;
using System.IO;

namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestBody : Basics.Context.Request.IHttpRequestBody
    {
        private readonly string _ContextId;
        private readonly HttpRequestForm _Form;
        private readonly HttpRequestFile _File;

        private readonly Basics.Context.Request.IHttpRequestHeader _Header;
        private readonly Net.NetworkStream _StreamEnclosure;

        private IO.BodyStream _ContentStream;

        public HttpRequestBody(string contextId, Basics.Context.Request.IHttpRequestHeader header, Net.NetworkStream streamEnclosure)
        {
            this._ContextId = contextId;

            this._Header = header;
            this._StreamEnclosure = streamEnclosure;

            this._Form = new HttpRequestForm();
            this._File = new HttpRequestFile();
        }

        public ParserResultTypes Parse()
        {
            ParserResultTypes parserResult = 
                this.CreateContentStream();
            if (parserResult != ParserResultTypes.Success) return parserResult;
            
            switch (this._Header.ContentType)
            {
                case "multipart/form-data":
                    Parser.MultipartFormData parserMultipartFormData =
                        new Parser.MultipartFormData(this._ContextId, this._Header, this._ContentStream);
                    return parserMultipartFormData.Parse(this._Form, this._File);
                case "application/x-www-form-urlencoded":
                    Parser.ApplicationXWwwFormUrlEncoded parserApplicationXWwwFormUrlEncoded =
                        new Parser.ApplicationXWwwFormUrlEncoded(this._Header, this._ContentStream);
                    return parserApplicationXWwwFormUrlEncoded.Parse(this._Form);
            }

            return ParserResultTypes.Success;
        }

        private ParserResultTypes CreateContentStream()
        {
            bool chunked =
                string.Compare(this._Header["Transfer-Encoding"], "chunked", StringComparison.OrdinalIgnoreCase) == 0;

            if (chunked)
            {
                if (this._Header.ContentLength > 0) return ParserResultTypes.BadRequest;
                    
                this._ContentStream = new IO.ChunkedStream(this._StreamEnclosure);
                return ParserResultTypes.Success;
            }

            if (this._Header.ContentLength > 0)
            {
                this._ContentStream = new IO.SizedStream(this._StreamEnclosure, this._Header.ContentLength);    
                return ParserResultTypes.Success;
            }

            this._ContentStream = new IO.EmptyStream();
            return ParserResultTypes.Success;
        }

        public Basics.Context.Request.IHttpRequestForm Form => this._Form;
        public Basics.Context.Request.IHttpRequestFile File => this._File;
        public Stream ContentStream => this._ContentStream;

        public void Conclude() => _ContentStream?.Conclude();
        
        internal void Dispose()
        {
            this._File.Dispose();
            
            _ContentStream?.Dispose();
        }
    }
}
