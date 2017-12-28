using System;

namespace Xeora.Web.Basics
{
    public interface IDomain : IDisposable
    {
        string[] IDAccessTree { get; }
        DomainInfo.DeploymentTypes DeploymentType { get; }

        IDomain Parent { get; }
        DomainInfo.DomainInfoCollection Children { get; }

        string ContentsVirtualPath { get; }
        ISettings Settings { get; }
        ILanguage Language { get; }
        IxService xService { get; }
        
        string Render(ServicePathInfo servicePathInfo, ControlResult.Message messageResult, string updateBlockControlID = null);
        string Render(string xeoraContent, ControlResult.Message messageResult, string updateBlockControlID = null);
        void ClearCache();
    }
}
