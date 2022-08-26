using System;

namespace Xeora.Web.Service.Context
{
    public class HttpContext : KeyValueCollection<string, object>, Basics.Context.IHttpContext
    {
        public HttpContext(
            string contextId,
            bool secure,
            Basics.Context.IHttpRequest request, 
            Basics.Context.IHttpResponse response,
            Basics.Session.IHttpSession session, 
            Basics.Application.IHttpApplication application)
        {
            this.UniqueId = contextId;
            this.HashCode = 
                this.GetOrCreateHashCode(ref request);
            this.Request = request;
            this.Response = response;
            this.Session = session;
            this.Application = application;
            
            ((HttpResponse)this.Response).SessionCookieRequested +=
                skip =>
                {
                    if (skip)
                        return null;

                    if (this.Session.Keys.Length == 0)
                        return null;

                    // Create SessionCookie
                    string sessionCookieKey = 
                        Basics.Configurations.Xeora.Session.CookieKey;
                    Basics.Context.IHttpCookieInfo sessionIdCookie =
                        this.Response.Header.Cookie.CreateNewCookie(sessionCookieKey);
                    sessionIdCookie.Value = this.Session.SessionId;
                    sessionIdCookie.Expires = this.Session.Expires;
                    sessionIdCookie.SameSite = secure ? Basics.Context.SameSiteTypes.None : Basics.Context.SameSiteTypes.Lax;
                    sessionIdCookie.Secure = secure;
                    sessionIdCookie.HttpOnly = true;

                    return sessionIdCookie;
                };
        }
        
        private string GetOrCreateHashCode(ref Basics.Context.IHttpRequest request)
        {
            string requestFilePath =
                request.Header.Url.RelativePath;

            int biIndex = 
                requestFilePath.IndexOf(Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation, StringComparison.InvariantCulture);
            if (biIndex > -1)
                requestFilePath = requestFilePath.Remove(0, biIndex + Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Length);

            System.Text.RegularExpressions.Match mR =
                System.Text.RegularExpressions.Regex.Match(requestFilePath, "\\d+/");

            if (mR.Success && mR.Index == 0)
                return mR.Value.Substring(0, mR.Length - 1);
            
            return this.GetHashCode().ToString().Replace("-", string.Empty);
        }
        
        public string UniqueId { get; }
        public Basics.Context.IHttpRequest Request { get; }
        public Basics.Context.IHttpResponse Response { get; }
        public Basics.Session.IHttpSession Session { get; }
        public Basics.Application.IHttpApplication Application { get; }

        public new void AddOrUpdate(string key, object value) =>
            base.AddOrUpdate(key, value);
        
        public string HashCode { get; }

        public void Dispose()
        {
            ((HttpRequest)this.Request).Dispose();
            ((HttpResponse)this.Response).Dispose();
        }
    }
}
