using System.Collections.Concurrent;

namespace Xeora.Web.Service.Context.Response
{
    public class HttpResponseStatusCodes : ConcurrentDictionary<short, string>
    {
        public HttpResponseStatusCodes()
        {
            TryAdd(100, "Continue");
            TryAdd(101, "Switching Protocols");
            TryAdd(102, "Processing");
            TryAdd(103, "Early Hints");

            TryAdd(200, "OK");
            TryAdd(201, "Created");
            TryAdd(202, "Accepted");
            TryAdd(203, "Non-Authoritative Information");
            TryAdd(204, "No Content");
            TryAdd(205, "Reset Content");
            TryAdd(206, "Partial Content");
            TryAdd(207, "Multi-Status");
            TryAdd(208, "Already Reported");
            TryAdd(218, "Has Inline Errors");
            TryAdd(226, "IM Used");

            TryAdd(300, "Multiple Choices");
            TryAdd(301, "Moved Permanently");
            TryAdd(302, "Found");
            TryAdd(303, "See Other");
            TryAdd(304, "Not Modified");
            TryAdd(305, "Use Proxy");
            TryAdd(306, "Switch Proxy");
            TryAdd(307, "Temporary Redirect");
            TryAdd(308, "Permanent Redirect");

            TryAdd(400, "Bad Request");
            TryAdd(401, "Unauthorized");
            TryAdd(402, "Payment Required");
            TryAdd(403, "Forbidden");
            TryAdd(404, "Not Found");
            TryAdd(405, "Method Not Allowed");
            TryAdd(406, "Not Acceptable");
            TryAdd(407, "Proxy Authentication Required");
            TryAdd(408, "Request Timeout");
            TryAdd(409, "Conflict");
            TryAdd(410, "Gone");
            TryAdd(411, "Length Required");
            TryAdd(412, "Precondition Failed");
            TryAdd(413, "Payload Too Large");
            TryAdd(414, "URI Too Long");
            TryAdd(415, "Unsupported Media Type");
            TryAdd(416, "Range Not Satisfiable");
            TryAdd(417, "Expectation Failed");
            TryAdd(418, "I'm a teapot");
            TryAdd(421, "Misdirected Request");
            TryAdd(422, "Unprocessable Entity");
            TryAdd(423, "Locked");
            TryAdd(424, "Failed Dependency");
            TryAdd(426, "Upgrade Required");
            TryAdd(428, "Precondition Required");
            TryAdd(429, "Too Many Requests");
            TryAdd(431, "Request Header Fields Too Large");
            TryAdd(451, "Unavailable For Legal Reasons");

            TryAdd(500, "Internal Server Error");
            TryAdd(501, "Not Implemented");
            TryAdd(502, "Bad Gateway");
            TryAdd(503, "Service Unavailable");
            TryAdd(504, "Gateway Timeout");
            TryAdd(505, "HTTP Version Not Supported");
            TryAdd(506, "Variant Also Negotiates");
            TryAdd(507, "Insufficient Storage");
            TryAdd(508, "Loop Detected");
            TryAdd(510, "Not Extended");
            TryAdd(511, "Network Authentication Required");
        }

        private static HttpResponseStatusCodes _statusCodes;
        public static HttpResponseStatusCodes StatusCodes =>
            HttpResponseStatusCodes._statusCodes ??
                       (HttpResponseStatusCodes._statusCodes = new HttpResponseStatusCodes());

        public string GetMessage(short code) => 
            !TryGetValue(code, out var message) ? "Unrecognised HTTP Code" : message;
    }
}
