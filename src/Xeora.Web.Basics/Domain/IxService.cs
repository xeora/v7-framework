using System.Collections;

namespace Xeora.Web.Basics.Domain
{
    public interface IxService
    {
        object ReadSessionVariable(string publicKey, string name);
        string CreateAuthentication(params DictionaryEntry[] items);
        RenderResult Render(string executeIn, string serviceID);
        RenderResult GenerateXML(object methodResult);
    }
}
