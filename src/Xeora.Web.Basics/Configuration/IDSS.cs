using System.Net;

namespace Xeora.Web.Basics.Configuration
{
    public interface IDSS
    {
        DSSServiceTypes ServiceType { get; }
        IPEndPoint ServiceEndPoint { get; }
    }
}
