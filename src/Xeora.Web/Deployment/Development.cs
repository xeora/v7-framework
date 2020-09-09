using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xeora.Web.Deployment
{
    internal class Development : IDeployment
    {
        public Development(string domainRootPath)
        {
            this.DomainRootPath = domainRootPath;
            this.CheckIntegrity();
        }

        private void CheckIntegrity()
        {
            if (!Directory.Exists(this.LanguagesRegistration) ||
                !Directory.Exists(this.TemplatesRegistration))
                throw new Exceptions.DeploymentException($"Domain {Global.SystemMessages.PATH_WRONGSTRUCTURE}");

            // Control Domain Language and Template Folders
            DirectoryInfo domainLanguagesDI =
                new DirectoryInfo(this.LanguagesRegistration);

            foreach (DirectoryInfo domainLanguageDI in domainLanguagesDI.GetDirectories())
            {
                if (!Directory.Exists(this.ContentsRegistration(domainLanguageDI.Name)))
                    throw new Exceptions.DeploymentException($"Domain {Global.SystemMessages.PATH_WRONGSTRUCTURE}");
            }
            // !--

            // -- Control Those System Essential Files are Exists! --
            string controlsXml =
                Path.Combine(this.TemplatesRegistration, "Controls.xml");
            string configurationXml =
                Path.Combine(this.TemplatesRegistration, "Configuration.xml");

            if (!File.Exists(configurationXml))
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND + "!");

            if (!File.Exists(controlsXml))
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND + "!");
            // !--
        }

        public string DomainRootPath { get; }
        public string ChildrenRegistration => Path.Combine(this.DomainRootPath, "Addons");
        public string ContentsRegistration(string languageId) => Path.Combine(this.DomainRootPath, "Contents", languageId);
        public string ExecutablesRegistration => Path.Combine(this.DomainRootPath, "Executables");
        public string TemplatesRegistration => Path.Combine(this.DomainRootPath, "Templates");
        public string LanguagesRegistration => Path.Combine(this.DomainRootPath, "Languages");

        public void ProvideContentFileStream(string languageId, string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', Path.DirectorySeparatorChar);
            if (requestedFilePath[0] == Path.DirectorySeparatorChar)
                requestedFilePath = requestedFilePath.Substring(1);

            string requestedFileFullPath =
                Path.Combine(this.ContentsRegistration(languageId), requestedFilePath);

            if (!File.Exists(requestedFileFullPath))
                throw new FileNotFoundException();

            outputStream = new FileStream(requestedFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public string ProvideTemplateContent(string serviceFullPath)
        {
            string templateFile =
                Path.Combine(this.TemplatesRegistration, $"{serviceFullPath}.xchtml");

            try
            {
                return Development.ReadFileAsString(templateFile);
            }
            catch (FileNotFoundException)
            {
                throw new Exceptions.DeploymentException(string.Format(Global.SystemMessages.TEMPLATE_NOTFOUND + "!", serviceFullPath));
            }
        }

        public string ProvideControlsContent()
        {
            string controlsXmlFile =
                Path.Combine(this.TemplatesRegistration, "Controls.xml");

            try
            {
                return Development.ReadFileAsString(controlsXmlFile);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, ex);
            }
        }

        public string ProvideConfigurationContent()
        {
            string configurationFile =
                Path.Combine(this.TemplatesRegistration, "Configuration.xml");

            try
            {
                return Development.ReadFileAsString(configurationFile);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, ex);
            }
        }

        public string ProvideLanguageContent(string languageId)
        {
            string languageFile =
                Path.Combine(this.LanguagesRegistration, $"{languageId}.xml");

            return Development.ReadFileAsString(languageFile);
        }

        private static string ReadFileAsString(string fileLocation)
        {
            byte[] buffer = new byte[102400];

            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                Encoding encoding =
                    Development.DetectEncoding(ref fileStream);
                StringBuilder fileContent = new StringBuilder();

                int rB;
                do
                {
                    rB = fileStream.Read(buffer, 0, buffer.Length);

                    if (rB > 0)
                        fileContent.Append(encoding.GetString(buffer, 0, rB));
                } while (rB != 0);

                return fileContent.ToString();
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public string[] Languages
        {
            get
            {
                List<string> languageIds =
                    new List<string>();

                DirectoryInfo languagesDI = 
                    new DirectoryInfo(this.LanguagesRegistration);

                foreach (FileInfo fI in languagesDI.GetFiles())
                    languageIds.Add(Path.GetFileNameWithoutExtension(fI.Name));

                return languageIds.ToArray();
            }
        }

        public bool Reload() => false;

        private static Encoding DetectEncoding(ref Stream inStream)
        {
            inStream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = 
                new byte[4];
            int bC = 
                inStream.Read(buffer, 0, buffer.Length);

            if (bC == 0)
                return Encoding.UTF8;

            if (bC >= 2 && buffer[0] == 254 && buffer[1] == 255)
            {
                inStream.Seek(2, SeekOrigin.Begin);

                return new UnicodeEncoding(true, true);
            }

            if (bC >= 2 && buffer[0] == 255 && buffer[1] == 254)
            {
                if (bC == 4 && buffer[2] == 0 && buffer[3] == 0)
                {
                    inStream.Seek(4, SeekOrigin.Begin);

                    return new UTF32Encoding(false, true);
                }

                inStream.Seek(2, SeekOrigin.Begin);

                return new UnicodeEncoding(false, true);
            }

            if (bC >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
            {
                inStream.Seek(3, SeekOrigin.Begin);

                return new UTF8Encoding();
            }

            if (bC == 4 && buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 254 && buffer[3] == 255)
            {
                inStream.Seek(4, SeekOrigin.Begin);

                return new UTF32Encoding(true, true);
            }

            inStream.Seek(0, SeekOrigin.Begin);

            return Encoding.UTF8;
        }
    }
}
