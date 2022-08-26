using System.IO;

namespace Xeora.Web.Service.Context.Request.Parser
{
    public class ApplicationXWwwFormUrlEncoded
    {
        private readonly Basics.Context.Request.IHttpRequestHeader _Header;
        private readonly IO.BodyStream _BodyStream;

        public ApplicationXWwwFormUrlEncoded(Basics.Context.Request.IHttpRequestHeader header, IO.BodyStream bodyStream)
        {
            this._Header = header;
            this._BodyStream = bodyStream;
        }

        internal ParserResultTypes Parse(HttpRequestForm form)
        {
            Stream contentStream = null;
            try
            {
                ParserResultTypes parserResult = 
                    this._BodyStream.ReadAllInto(ref contentStream);
                if (parserResult != ParserResultTypes.Success) return parserResult;
                
                contentStream.Seek(0, SeekOrigin.Begin);

                StreamReader sR = this._Header.ContentEncoding != null 
                    ? new StreamReader(contentStream, this._Header.ContentEncoding, true) 
                    : new StreamReader(contentStream, true);

                string formContent = sR.ReadToEnd();
                formContent = formContent.Trim();

                if (string.IsNullOrEmpty(formContent))
                    return ParserResultTypes.Success;

                string[] keyValues = formContent.Split('&');

                foreach (string keyValue in keyValues)
                {
                    int equalsIndex = keyValue.IndexOf('=');
                    string key, value = string.Empty;

                    if (equalsIndex == -1)
                        key = keyValue;
                    else
                    {
                        key = keyValue.Substring(0, equalsIndex);
                        value = keyValue.Substring(equalsIndex + 1);

                        value = System.Web.HttpUtility.UrlDecode(value);
                    }

                    if (form.ContainsKey(key))
                        value = $"{form[key]},{value}";

                    form.AddOrUpdate(key, value);
                }

                return ParserResultTypes.Success;
            }
            finally
            {
                contentStream?.Dispose();
            }
        }
    }
}
