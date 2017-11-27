namespace Xeora.Web.Service.Context
{
    public class HttpRequestFile : KeyValueCollection<string, Basics.Context.IHttpRequestFileInfo>, Basics.Context.IHttpRequestFile
    {
        internal void Dispose()
        {
            foreach (string key in this.Keys)
                ((HttpRequestFileInfo)base[key]).Dispose();
        }
    }
}
