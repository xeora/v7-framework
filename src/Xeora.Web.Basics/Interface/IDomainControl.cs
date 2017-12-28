using System.IO;

namespace Xeora.Web.Basics
{
    public interface IDomainControl
    {
        string SiteTitle { get; set; }
        string SiteIconURL { get; set; }
        IMetaRecordCollection MetaRecord { get; }

        IDomain Domain { get; }
        void ProvideFileStream(string requestedFilePath, out Stream outputStream);
        void PushLanguageChange(string languageID);
        URLMapping.ResolvedMapped QueryURLResolver(string requestFilePath);
        DomainInfo.DomainInfoCollection GetAvailableDomains();
    }
}
