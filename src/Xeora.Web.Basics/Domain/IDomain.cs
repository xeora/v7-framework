using System.IO;

namespace Xeora.Web.Basics.Domain
{
    public interface IDomain
    {
        string[] IdAccessTree { get; }
        Info.DeploymentTypes DeploymentType { get; }

        IDomain Parent { get; }
        Info.DomainCollection Children { get; }

        string ContentsVirtualPath { get; }
        ISettings Settings { get; }
        ILanguages Languages { get; }
        IxService xService { get; }

        /// <summary>
        /// Provides the domain contents file stream
        /// </summary>
        /// <param name="requestedFilePath">File path and name to read</param>
        /// <param name="outputStream">Output stream</param>
        void ProvideFileStream(string requestedFilePath, out Stream outputStream);

        RenderResult Render(ServiceDefinition serviceDefinition, ControlResult.Message messageResult, string[] updateBlockControlIdStack = null);
        RenderResult Render(string xeoraContent, ControlResult.Message messageResult, string[] updateBlockControlIdStack = null);
    }
}
