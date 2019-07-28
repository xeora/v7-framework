namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestFile : KeyValueCollection<string, Basics.Context.Request.IHttpRequestFileInfo>, Basics.Context.Request.IHttpRequestFile
    {
        internal void Dispose()
        {
            foreach (string key in this.Keys)
                ((HttpRequestFileInfo)base[key]).Dispose();
        }
    }
}
