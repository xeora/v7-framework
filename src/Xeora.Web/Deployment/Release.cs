using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Xeora.Web.Deployment
{
    internal class Release : IDeployment
    {
        public Release(string domainRootPath)
        {
            this.DomainRootPath = domainRootPath;
            this.Extractor = new Extractor(this.DomainRootPath);
            this.CheckIntegrity();
        }

        private void CheckIntegrity()
        {
            // -- Control Those System Essential Files are Exists! --
            FileEntry controlsXmlFileEntry =
                this.Extractor.Get(this.TemplatesRegistration, "Controls.xml");
            FileEntry configurationFileEntry =
                this.Extractor.Get(this.TemplatesRegistration, "Configuration.xml");

            if (configurationFileEntry.Index == -1)
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND + "!");

            if (controlsXmlFileEntry.Index == -1)
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND + "!");
            // !--
        }

        public string DomainRootPath { get; }
        public string ChildrenRegistration => Path.Combine(this.DomainRootPath, "Addons");
        public string ContentsRegistration(string languageId) => $"\\Contents\\{languageId}\\";
        public string ExecutablesRegistration => Path.Combine(this.DomainRootPath, "Executables");
        public string TemplatesRegistration => "\\Templates\\";
        public string LanguagesRegistration => "\\Languages\\";

        private Extractor Extractor { get; }

        public void ProvideContentFileStream(string languageId, string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', '\\');
            if (requestedFilePath[0] == '\\')
                requestedFilePath = requestedFilePath.Substring(1);

            string requestPath = string.Empty;
            string requestFile = requestedFilePath;

            int lastIndex = requestedFilePath.LastIndexOf('\\');
            if (lastIndex > -1)
            {
                requestPath = requestedFilePath.Substring(0, lastIndex + 1);
                requestFile = requestedFilePath.Substring(lastIndex + 1);
            }

            FileEntry fileEntry =
                this.Extractor.Get(
                    string.Concat(
                        this.ContentsRegistration(languageId),
                        requestPath
                    ),
                    requestFile
                );

            if (fileEntry.Index == -1)
                throw new FileNotFoundException();

            outputStream = new MemoryStream();

            RequestResults requestResult =
                this.Extractor.Read(fileEntry.Index, fileEntry.CompressedLength, ref outputStream);

            switch (requestResult)
            {
                case RequestResults.ContentNotExists:
                    throw new FileNotFoundException();
                case RequestResults.PasswordError:
                    throw new Exceptions.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
            }
        }

        public string ProvideTemplateContent(string serviceFullPath)
        {
            // Compiled Xeora Content File Index header separates
            // PATH and FILE differently. serviceFullPath contain filename with 
            // path name which is not fitting Index header records.
            string registrationPath = this.TemplatesRegistration;
            string fileName = serviceFullPath;

            if (fileName.IndexOf('/') > 0)
            {
                int idx = fileName.LastIndexOf('/');

                registrationPath = $"{registrationPath}{fileName.Substring(0, idx + 1).Replace('/', '\\')}";
                fileName = fileName.Substring(idx + 1);
            }
            // !--

            FileEntry fileEntry =
                this.Extractor.Get(
                    registrationPath, $"{fileName}.xchtml");

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exceptions.DeploymentException(string.Format(Global.SystemMessages.TEMPLATE_NOTFOUND + "!", serviceFullPath));
            }
        }

        public string ProvideControlsContent()
        {
            FileEntry fileEntry =
                this.Extractor.Get(this.TemplatesRegistration, "Controls.xml");

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, new FileNotFoundException());
            }
        }

        public string ProvideConfigurationContent()
        {
            FileEntry fileEntry =
                this.Extractor.Get(this.TemplatesRegistration, "Configuration.xml");

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exceptions.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, new FileNotFoundException());
            }
        }

        public string ProvideLanguageContent(string languageId)
        {
            FileEntry fileEntry =
                this.Extractor.Get(this.LanguagesRegistration, $"{languageId}.xml");

            return fileEntry.Index == -1 ? string.Empty : this.ReadFileAsString(fileEntry);
        }

        private string ReadFileAsString(FileEntry fileEntry)
        {
            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();

                RequestResults requestResult =
                    this.Extractor.Read(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                switch (requestResult)
                {
                    case RequestResults.Authenticated:
                        StreamReader sR = new StreamReader(contentStream);

                        return sR.ReadToEnd();
                    case RequestResults.ContentNotExists:
                        throw new FileNotFoundException();
                    case RequestResults.PasswordError:
                        throw new Exceptions.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                }

                return string.Empty;
            }
            finally
            {
                contentStream?.Close();
            }
        }

        public string[] Languages
        {
            get
            {
                List<string> languageIds =
                    new List<string>();

                IEnumerable<FileEntry> searchResult =
                    this.Extractor.Search(this.LanguagesRegistration, string.Empty);

                foreach (FileEntry fileEntry in searchResult)
                {
                    languageIds.Add(
                        Path.GetFileNameWithoutExtension(fileEntry.FileName));
                }

                return languageIds.ToArray();
            }
        }
    }
}
