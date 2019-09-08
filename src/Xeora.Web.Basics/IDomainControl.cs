using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Info;

namespace Xeora.Web.Basics
{
    public interface IDomainControl
    {
        string SiteTitle { get; set; }
        string SiteIconUrl { get; set; }
        IMetaRecordCollection MetaRecord { get; }

        ICryptography Cryptography { get; }
        IDomain Domain { get; }

        Mapping.ResolutionResult ResolveUrl(string requestFilePath);
        DomainCollection GetAvailableDomains();
    }
}
