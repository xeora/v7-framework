using System.IO;

namespace Xeora.Web.Deployment
{
    internal interface IDeployment
    {
        string DomainRootPath { get; }

        string ChildrenRegistration { get; }
        string ContentsRegistration(string languageID);
        string ExecutablesRegistration { get; }
        string TemplatesRegistration { get; }
        string LanguagesRegistration { get; }

        string[] Languages { get; }

        void ProvideContentFileStream(string languageID, string requestedFilePath, out Stream outputStream);
        string ProvideTemplateContent(string serviceFullPath);
        string ProvideControlsContent();
        string ProvideConfigurationContent();
        string ProvideLanguageContent(string languageID);

        bool Reload();
    }
}
