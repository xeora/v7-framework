using System;
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
            this.Decompiler = new Decompiler(this.DomainRootPath);
            this.CheckIntegrity();
        }

        private void CheckIntegrity()
        {
            // -- Control Those System Essential Files are Exists! --
            FileEntry controlsXMLFileEntry =
                this.Decompiler.Get(this.TemplatesRegistration, "Controls.xml");
            FileEntry configurationFileEntry =
                this.Decompiler.Get(this.TemplatesRegistration, "Configuration.xml");

            if (configurationFileEntry.Index == -1)
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND + "!");

            if (controlsXMLFileEntry.Index == -1)
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND + "!");
            // !--
        }

        public string DomainRootPath { get; private set; }
        public string ChildrenRegistration => Path.Combine(this.DomainRootPath, "Addons");
        public string ContentsRegistration(string languageID) => string.Format("\\Contents\\{0}\\", languageID);
        public string ExecutablesRegistration => Path.Combine(this.DomainRootPath, "Executables");
        public string TemplatesRegistration => "\\Templates\\";
        public string LanguagesRegistration => "\\Languages\\";

        private Decompiler Decompiler { get; set; }

        public void ProvideContentFileStream(string languageID, string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', Path.DirectorySeparatorChar);
            if (requestedFilePath[0] == Path.DirectorySeparatorChar)
                requestedFilePath = requestedFilePath.Substring(1);

            FileEntry fileEntry =
                this.Decompiler.Get(
                    Path.Combine(
                        this.ContentsRegistration(languageID),
                        requestedFilePath.Replace(Path.GetFileName(requestedFilePath), string.Empty)
                    ),
                    Path.GetFileName(requestedFilePath)
                );

            if (fileEntry.Index == -1)
                return;

            outputStream = new MemoryStream();

            RequestResults requestResult =
                this.Decompiler.Read(fileEntry.Index, fileEntry.CompressedLength, ref outputStream);

            switch (requestResult)
            {
                case RequestResults.ContentNotExists:
                    throw new FileNotFoundException();
                case RequestResults.PasswordError:
                    throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
            }
        }

        public string ProvideTemplateContent(string serviceFullPath)
        {
            FileEntry fileEntry =
                this.Decompiler.Get(
                    this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exception.DeploymentException(string.Format(Global.SystemMessages.TEMPLATE_NOTFOUND + "!", serviceFullPath));
            }
        }

        public string ProvideControlsContent()
        {
            FileEntry fileEntry =
                this.Decompiler.Get(this.TemplatesRegistration, "Controls.xml");

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, new FileNotFoundException());
            }
        }

        public string ProvideConfigurationContent()
        {
            FileEntry fileEntry =
                this.Decompiler.Get(this.TemplatesRegistration, "Configuration.xml");

            if (fileEntry.Index == -1)
                return string.Empty;

            try
            {
                return this.ReadFileAsString(fileEntry);
            }
            catch (FileNotFoundException)
            {
                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, new FileNotFoundException());
            }
        }

        public string ProvideLanguageContent(string languageID)
        {
            FileEntry fileEntry =
                this.Decompiler.Get(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

            if (fileEntry.Index == -1)
                return string.Empty;

            return this.ReadFileAsString(fileEntry);
        }

        private string ReadFileAsString(FileEntry fileEntry)
        {
            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();

                RequestResults requestResult =
                    this.Decompiler.Read(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                switch (requestResult)
                {
                    case RequestResults.Authenticated:
                        StreamReader sR = new StreamReader(contentStream);

                        return sR.ReadToEnd();
                    case RequestResults.ContentNotExists:
                        throw new FileNotFoundException();
                    case RequestResults.PasswordError:
                        throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                }

                return string.Empty;
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    GC.SuppressFinalize(contentStream);
                }
            }
        }

        public string[] Languages
        {
            get
            {
                List<string> languageIDs =
                    new List<string>();

                FileEntry[] searchResult =
                    this.Decompiler.Search(this.LanguagesRegistration, string.Empty);

                foreach (FileEntry fileEntry in searchResult)
                {
                    languageIDs.Add(
                        Path.GetFileNameWithoutExtension(fileEntry.FileName));
                }

                return languageIDs.ToArray();
            }
        }

        public bool Reload() => this.Decompiler.Reload();
    }
}
