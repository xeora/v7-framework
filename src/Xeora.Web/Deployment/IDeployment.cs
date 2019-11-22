using System.IO;

namespace Xeora.Web.Deployment
{
    internal interface IDeployment
    {
        string DomainRootPath { get; }

        string ChildrenRegistration { get; }
        string ContentsRegistration(string languageId);
        string ExecutablesRegistration { get; }
        string TemplatesRegistration { get; }
        string LanguagesRegistration { get; }

        string[] Languages { get; }

        void ProvideContentFileStream(string languageId, string requestedFilePath, out Stream outputStream);
        string ProvideTemplateContent(string serviceFullPath);
        string ProvideControlsContent();
        string ProvideConfigurationContent();
        string ProvideLanguageContent(string languageId);
    }
}
