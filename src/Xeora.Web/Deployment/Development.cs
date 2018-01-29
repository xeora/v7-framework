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
                throw new Exception.DeploymentException(string.Format("Domain {0}", Global.SystemMessages.PATH_WRONGSTRUCTURE));

            // Control Domain Language and Template Folders
            DirectoryInfo domainLanguagesDI =
                new DirectoryInfo(this.LanguagesRegistration);

            foreach (DirectoryInfo domainLanguageDI in domainLanguagesDI.GetDirectories())
            {
                if (!Directory.Exists(this.ContentsRegistration(domainLanguageDI.Name)))
                    throw new Exception.DeploymentException(string.Format("Domain {0}", Global.SystemMessages.PATH_WRONGSTRUCTURE));
            }
            // !--

            // -- Control Those System Essential Files are Exists! --
            string controlsXML =
                Path.Combine(this.TemplatesRegistration, "Controls.xml");
            string configurationXML =
                Path.Combine(this.TemplatesRegistration, "Configuration.xml");

            if (!File.Exists(configurationXML))
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND + "!");

            if (!File.Exists(controlsXML))
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND + "!");
            // !--
        }

        public string DomainRootPath { get; private set; }
        public string ChildrenRegistration => Path.Combine(this.DomainRootPath, "Addons");
        public string ContentsRegistration(string languageID) => Path.Combine(this.DomainRootPath, "Contents", languageID);
        public string ExecutablesRegistration => Path.Combine(this.DomainRootPath, "Executables");
        public string TemplatesRegistration => Path.Combine(this.DomainRootPath, "Templates");
        public string LanguagesRegistration => Path.Combine(this.DomainRootPath, "Languages");

        public void ProvideContentFileStream(string languageID, string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', Path.DirectorySeparatorChar);
            if (requestedFilePath[0] == Path.DirectorySeparatorChar)
                requestedFilePath = requestedFilePath.Substring(1);

            string requestedFileFullPath =
                Path.Combine(this.ContentsRegistration(languageID), requestedFilePath);

            if (!File.Exists(requestedFileFullPath))
                throw new FileNotFoundException();

            outputStream = new FileStream(requestedFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public string ProvideTemplateContent(string serviceFullPath)
        {
            string templateFile =
                Path.Combine(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

            try
            {
                return this.ReadFileAsString(templateFile);
            }
            catch (FileNotFoundException)
            {
                throw new Exception.DeploymentException(string.Format(Global.SystemMessages.TEMPLATE_NOFOUND + "!", serviceFullPath));
            }
        }

        public string ProvideControlsContent()
        {
            string controlsXMLFile =
                Path.Combine(this.TemplatesRegistration, "Controls.xml");

            try
            {
                return this.ReadFileAsString(controlsXMLFile);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, ex);
            }
        }

        public string ProvideConfigurationContent()
        {
            string configurationFile =
                Path.Combine(this.TemplatesRegistration, "Configuration.xml");

            try
            {
                return this.ReadFileAsString(configurationFile);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, ex);
            }
        }

        public string ProvideLanguageContent(string languageID)
        {
            string languageFile =
                Path.Combine(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

            return this.ReadFileAsString(languageFile);
        }

        private string ReadFileAsString(string fileLocation)
        {
            byte[] buffer = new byte[102400];
            int rB = 0;

            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                Encoding encoding =
                    this.DetectEncoding(ref fileStream);
                StringBuilder fileContent = new StringBuilder();

                do
                {
                    rB = fileStream.Read(buffer, 0, buffer.Length);

                    if (rB > 0)
                        fileContent.Append(encoding.GetString(buffer, 0, rB));
                } while (rB != 0);

                return fileContent.ToString();
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        public string[] Languages
        {
            get
            {
                List<string> languageIDs =
                    new List<string>();

                DirectoryInfo languagesDI = 
                    new DirectoryInfo(this.LanguagesRegistration);

                foreach (FileInfo tFI in languagesDI.GetFiles())
                    languageIDs.Add(Path.GetFileNameWithoutExtension(tFI.Name));

                return languageIDs.ToArray();
            }
        }

        public bool Reload() => false;

        private Encoding DetectEncoding(ref Stream inStream)
        {
            inStream.Seek(0, SeekOrigin.Begin);

            int bC = 0;
            byte[] buffer = new byte[4];

            bC = inStream.Read(buffer, 0, buffer.Length);

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
