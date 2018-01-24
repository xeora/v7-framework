using System.Collections;

namespace Xeora.Web.Basics.Domain
{
    public interface IxService
    {
        object ReadSessionVariable(string publicKey, string name);
        string CreateAuthentication(params DictionaryEntry[] items);
        string Render(string executeIn, string serviceID);
        string GenerateXML(object methodResult);
    }
}
