using System.Net;

namespace Xeora.Web.Basics.Configuration
{
    public interface IDss
    {
        DssServiceTypes ServiceType { get; }
        IPEndPoint ServiceEndPoint { get; }
    }
}
