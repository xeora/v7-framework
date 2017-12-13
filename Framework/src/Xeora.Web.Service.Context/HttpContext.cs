using Xeora.Web.Service.Application;
using Xeora.Web.Service.Session;

namespace Xeora.Web.Service.Context
{
    public class HttpContext : KeyValueCollection<string, object>, Basics.Context.IHttpContext
    {
        private Basics.Session.IHttpSession _Session = null;

        public HttpContext(string contextID, ref Basics.Context.IHttpRequest request)
        {
            string sessionCookieKey = Basics.Configurations.Xeora.Session.CookieKey;

            this.Request = request;
            this.Response = new HttpResponse(contextID);

            string sessionID = string.Empty;
            Basics.Context.IHttpCookieInfo sessionIDCookie =
                this.Request.Header.Cookie[sessionCookieKey];
            if (sessionIDCookie != null)
            {
                sessionID = sessionIDCookie.Value;

                // Remove sessioncookie from the request object
                ((HttpCookie)this.Request.Header.Cookie).Remove(sessionCookieKey);
            }

            SessionManager.Current.Acquire(
                this.Request.RemoteAddr,
                sessionID,
                out this._Session);

            ((HttpResponse)this.Response).SessionCookieRequested +=
                () =>
                {
                    if (string.Compare(sessionID, this._Session.SessionID) == 0)
                        return null;

                    string contentType =
                        this.Response.Header["Content-Type"];
                    if (string.IsNullOrEmpty(contentType) ||
                        contentType.IndexOf("text/html") == -1)
                        return null;

                    if (this._Session.Keys.Length == 0)
                        return null;

                    // Create SessionCookie
                    sessionIDCookie =
                        this.Response.Header.Cookie.CreateNewCookie(sessionCookieKey);
                    sessionIDCookie.Value = this._Session.SessionID;
                    sessionIDCookie.HttpOnly = true;

                    return sessionIDCookie;
                };

            this.Application = ApplicationContainer.Current;
        }

        public Basics.Context.IHttpRequest Request { get; private set; }
        public Basics.Context.IHttpResponse Response { get; private set; }
        public Basics.Session.IHttpSession Session => this._Session;
        public Basics.Application.IHttpApplication Application { get; private set; }

        public new void AddOrUpdate(string key, object value) =>
            base.AddOrUpdate(key, value);

        private string _HashCode = string.Empty;
        public string HashCode
        {
            get
            {
                if (!string.IsNullOrEmpty(this._HashCode))
                    return this._HashCode;

                string RequestFilePath =
                    this.Request.Header.URL.RelativePath;

                int biIndex = RequestFilePath.IndexOf(Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);
                if (biIndex > -1)
                    RequestFilePath = RequestFilePath.Remove(0, biIndex + Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Length);

                System.Text.RegularExpressions.Match mR =
                    System.Text.RegularExpressions.Regex.Match(RequestFilePath, "\\d+/");

                if (mR.Success && mR.Index == 0)
                    this._HashCode = mR.Value.Substring(0, mR.Value.Length - 1);
                else
                {
                    this._HashCode = this.GetHashCode().ToString();
                    this._HashCode = this._HashCode.Replace("-", string.Empty);
                }

                return this._HashCode;
            }
        }

        public void Dispose()
        {
            ((HttpResponse)this.Response).Dispose();
            ((HttpRequest)this.Request).Dispose();
        }
    }
}
