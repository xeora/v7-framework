using System.Collections;

namespace Xeora.Web.Basics
{
    public interface IxService
    {
        object ReadSessionVariable(string PublicKey, string name);
        string CreatexServiceAuthentication(params DictionaryEntry[] dItems);
        string RenderxService(string ExecuteIn, string ServiceID);
        string GeneratexServiceXML(object MethodResult);
    }
}
