using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Info;

namespace Xeora.Web.Basics
{
    public interface IDomainControl
    {
        string SiteTitle { get; set; }
        string SiteIconURL { get; set; }
        IMetaRecordCollection MetaRecord { get; }

        IDomain Domain { get; }
        IRenderEngine RenderEngine { get; }

        Mapping.ResolutionResult ResolveURL(string requestFilePath);
        DomainCollection GetAvailableDomains();
    }
}
