using System.Collections.Concurrent;

namespace Xeora.Web.Service.Context
{
    public class HttpResponseStatusCodes : ConcurrentDictionary<short, string>
    {
        public HttpResponseStatusCodes()
        {
            base.TryAdd(100, "Continue");
            base.TryAdd(101, "Switching Protocols");
            base.TryAdd(102, "Processing");
            base.TryAdd(103, "Early Hints");

            base.TryAdd(200, "OK");
            base.TryAdd(201, "Created");
            base.TryAdd(202, "Accepted");
            base.TryAdd(203, "Non-Authoritative Information");
            base.TryAdd(204, "No Content");
            base.TryAdd(205, "Reset Content");
            base.TryAdd(206, "Partial Content");
            base.TryAdd(207, "Multi-Status");
            base.TryAdd(208, "Already Reported");
            base.TryAdd(226, "IM Used");

            base.TryAdd(300, "Multiple Choices");
            base.TryAdd(301, "Moved Permanently");
            base.TryAdd(302, "Found");
            base.TryAdd(303, "See Other");
            base.TryAdd(304, "Not Modified");
            base.TryAdd(305, "Use Proxy");
            base.TryAdd(306, "Switch Proxy");
            base.TryAdd(307, "Temporary Redirect");
            base.TryAdd(308, "Permanent Redirect");

            base.TryAdd(400, "Bad Request");
            base.TryAdd(401, "Unauthorized");
            base.TryAdd(402, "Payment Required");
            base.TryAdd(403, "Forbidden");
            base.TryAdd(404, "Not Found");
            base.TryAdd(405, "Method Not Allowed");
            base.TryAdd(406, "Not Acceptable");
            base.TryAdd(407, "Proxy Authentication Required");
            base.TryAdd(408, "Request Timeout");
            base.TryAdd(409, "Conflict");
            base.TryAdd(410, "Gone");
            base.TryAdd(411, "Length Required");
            base.TryAdd(412, "Precondition Failed");
            base.TryAdd(413, "Payload Too Large");
            base.TryAdd(414, "URI Too Long");
            base.TryAdd(415, "Unsupported Media Type");
            base.TryAdd(416, "Range Not Satisfiable");
            base.TryAdd(417, "Expectation Failed");
            base.TryAdd(418, "I'm a teapot");
            base.TryAdd(421, "Misdirected Request");
            base.TryAdd(422, "Unprocessable Entity");
            base.TryAdd(423, "Locked");
            base.TryAdd(424, "Failed Dependency");
            base.TryAdd(426, "Upgrade Required");
            base.TryAdd(428, "Precondition Required");
            base.TryAdd(429, "Too Many Requests");
            base.TryAdd(431, "Request Header Fields Too Large");
            base.TryAdd(451, "Unavailable For Legal Reasons");

            base.TryAdd(500, "Internal Server Error");
            base.TryAdd(501, "Not Implemented");
            base.TryAdd(502, "Bad Gateway");
            base.TryAdd(503, "Service Unavailable");
            base.TryAdd(504, "Gateway Timeout");
            base.TryAdd(505, "HTTP Version Not Supported");
            base.TryAdd(506, "Variant Also Negotiates");
            base.TryAdd(507, "Insufficient Storage");
            base.TryAdd(508, "Loop Detected");
            base.TryAdd(510, "Not Extended");
            base.TryAdd(511, "Network Authentication Required");
        }

        private static HttpResponseStatusCodes _StatusCodes = null;
        public static HttpResponseStatusCodes StatusCodes
        {
            get 
            {
                if (HttpResponseStatusCodes._StatusCodes == null)
                    HttpResponseStatusCodes._StatusCodes = new HttpResponseStatusCodes();

                return HttpResponseStatusCodes._StatusCodes;
            }
        }

        public string GetMessage(short code)
        {
            string message = string.Empty;
           if (!base.TryGetValue(code, out message))
                return "Unrecognised HTTP Code";

            return message;
        }
    }
}
