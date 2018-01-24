using System;
using System.IO;
using System.Security;
using System.Text;

namespace Xeora.Web.Deployment
{
    public abstract class DeploymentBase
    {
        public abstract void Dispose();
        public abstract Basics.Domain.ISettings Settings { get; }
        public abstract Basics.Domain.ILanguages Languages { get; }
        public abstract Basics.Domain.IControls Controls { get; }
        public abstract Basics.Domain.IxService xService { get; }
        public abstract Basics.Domain.Info.DomainCollection Children { get; }

        protected DeploymentBase(string[] domainIDAccessTree)
        {
            this.DomainIDAccessTree = domainIDAccessTree;

            if (this.DomainIDAccessTree == null || 
                this.DomainIDAccessTree.Length == 0)
                throw new Exception.DeploymentException(Global.SystemMessages.IDMUSTBESET);

            this.WorkingRoot = 
                Path.GetFullPath(
                    Path.Combine(
                        Basics.Configurations.Xeora.Application.Main.PhysicalRoot, 
                        Basics.Configurations.Xeora.Application.Main.ApplicationRoot.FileSystemImplementation, 
                        "Domains", 
                        this.CreateDomainAccessPathString()
                    )
                );

            if (!Directory.Exists(this.WorkingRoot))
                throw new Exception.DomainNotExistsException(
                    Global.SystemMessages.PATH_NOTEXISTS, 
                    new DirectoryNotFoundException(string.Format("WorkingPath: {0}", this.WorkingRoot))
                );

            string releaseTestPath = 
                Path.Combine(this.WorkingRoot, "Content.xeora");

            if (File.Exists(releaseTestPath))
            {
                this.DeploymentType = Basics.Domain.Info.DeploymentTypes.Release;

                this.Decompiler = new DomainDecompiler(this.WorkingRoot);
            }
            else
                this.DeploymentType = Basics.Domain.Info.DeploymentTypes.Development;
        }

        public string[] DomainIDAccessTree { get; private set; }
        public Basics.Domain.Info.DeploymentTypes DeploymentType { get; private set; }

        internal DomainDecompiler Decompiler { get; private set; }

        protected string WorkingRoot { get; private set; }
        protected string ExecutablesPath => Path.Combine(this.WorkingRoot, "Executables");

        protected string TemplatesRegistration
        {
            get
            {
                if (this.DeploymentType == Basics.Domain.Info.DeploymentTypes.Release)
                    return "\\Templates\\";
                
                return Path.Combine(this.WorkingRoot, "Templates");
            }
        }

        protected string DomainContentsRegistration(string languageID)
        {
            if (this.DeploymentType == Basics.Domain.Info.DeploymentTypes.Release)
                return string.Format("\\Contents\\{0}\\", languageID);
                
            return Path.Combine(this.WorkingRoot, "Contents", languageID);
        }

        protected string LanguagesRegistration
        {
            get
            {
                if (this.DeploymentType == Basics.Domain.Info.DeploymentTypes.Release)
                    return "\\Languages\\";
                
                return Path.Combine(this.WorkingRoot, "Languages");
            }
        }

        protected string ChildrenRootPath => Path.Combine(this.WorkingRoot, "Addons");

        private string CreateDomainAccessPathString()
        {
            string rDomainAccessPath = this.DomainIDAccessTree[0];

            for (int iC = 1; iC < this.DomainIDAccessTree.Length; iC++)
                rDomainAccessPath = Path.Combine(rDomainAccessPath, "Addons", this.DomainIDAccessTree[iC]);

            return rDomainAccessPath;
        }

        private Encoding DetectEncoding(ref Stream inStream)
        {
            inStream.Seek(0, SeekOrigin.Begin);

            int bC = 0;
            byte[] buffer = new byte[4];

            bC = inStream.Read(buffer, 0, buffer.Length);

            if (bC > 0)
            {
                if (bC >= 2 && buffer[0] == 254 && buffer[1] == 255)
                {
                    inStream.Seek(2, SeekOrigin.Begin);

                    return new UnicodeEncoding(true, true);
                }
                else if (bC >= 2 && buffer[0] == 255 && buffer[1] == 254)
                {
                    if (bC == 4 && buffer[2] == 0 && buffer[3] == 0)
                    {
                        inStream.Seek(4, SeekOrigin.Begin);

                        return new UTF32Encoding(false, true);
                    }

                    inStream.Seek(2, SeekOrigin.Begin);

                    return new UnicodeEncoding(false, true);
                }
                else if (bC >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
                {
                    inStream.Seek(3, SeekOrigin.Begin);

                    return new UTF8Encoding();
                }
                else if (bC == 4 && buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 254 && buffer[3] == 255)
                {
                    inStream.Seek(4, SeekOrigin.Begin);

                    return new UTF32Encoding(true, true);
                }

                inStream.Seek(0, SeekOrigin.Begin);
            }

            return Encoding.UTF8;
        }

        public bool CheckTemplateExists(string serviceFullPath)
        {
            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    return File.Exists(
                        Path.Combine(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath)));
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry = 
                        this.Decompiler.GetFileEntry(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    return fileEntry.Index > -1;
            }

            return false;
        }

        public virtual string ProvideTemplateContent(string serviceFullPath)
        {
            string rTemplateContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    string templateFile = 
                        Path.Combine(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder templateContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                templateContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rTemplateContent = templateContent.ToString();
                    }
                    catch (System.Exception)
                    {
                        rTemplateContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry = 
                        this.Decompiler.GetFileEntry(
                            this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    if (fileEntry.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        DomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case DomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rTemplateContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case DomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rTemplateContent;
        }

        public string ProvideLanguageContent(string languageID)
        {
            string rLanguageContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    string languageFile = 
                        Path.Combine(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder languageContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(languageFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                languageContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rLanguageContent = languageContent.ToString();
                    }
                    catch (FileNotFoundException)
                    {
                        if (string.Compare(languageID, this.Settings.Configurations.DefaultLanguage) != 0)
                            rLanguageContent = this.ProvideLanguageContent(this.Settings.Configurations.DefaultLanguage);
                    }
                    catch (System.Exception)
                    {
                        rLanguageContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry = 
                        this.Decompiler.GetFileEntry(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

                    if (fileEntry.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        DomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case DomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rLanguageContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case DomainDecompiler.RequestResults.ContentNotExists:
                                if (string.Compare(languageID, this.Settings.Configurations.DefaultLanguage) != 0)
                                    rLanguageContent = this.ProvideLanguageContent(this.Settings.Configurations.DefaultLanguage);

                                break;
                            case DomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rLanguageContent;
        }

        public void ProvideContentFileStream(string languageID, string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', Path.DirectorySeparatorChar);
            if (requestedFilePath[0] == Path.DirectorySeparatorChar)
                requestedFilePath = requestedFilePath.Substring(1);

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    string requestedFileFullPath =
                        Path.Combine(this.DomainContentsRegistration(languageID), requestedFilePath);

                    if (File.Exists(requestedFileFullPath))
                    {
                        try
                        {
                            outputStream = new FileStream(requestedFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                            return;
                        }
                        catch (System.Exception)
                        {
                            outputStream = null;
                        }
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry =
                        this.Decompiler.GetFileEntry(
                            Path.Combine(
                                this.DomainContentsRegistration(languageID),
                                requestedFilePath.Replace(Path.GetFileName(requestedFilePath), string.Empty)
                            ),
                            Path.GetFileName(requestedFilePath)
                        );

                    if (fileEntry.Index > -1)
                    {
                        outputStream = new MemoryStream();

                        DomainDecompiler.RequestResults requestResult =
                            this.Decompiler.ReadFile(fileEntry.Index, fileEntry.CompressedLength, ref outputStream);

                        if (requestResult == DomainDecompiler.RequestResults.PasswordError)
                            throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                    }

                    break;
            }
        }
        
        public string ProvideConfigurationContent()
        {
            string rConfigurationContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    string configurationFile = Path.Combine(this.TemplatesRegistration, "Configuration.xml");

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder configurationContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(configurationFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                configurationContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rConfigurationContent = configurationContent.ToString();
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, ex);
                    }
                    catch (System.Exception)
                    {
                        rConfigurationContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry = 
                        this.Decompiler.GetFileEntry(this.TemplatesRegistration, "Configuration.xml");

                    if (fileEntry.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        DomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case DomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rConfigurationContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case DomainDecompiler.RequestResults.ContentNotExists:
                                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, new FileNotFoundException());
                            case DomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rConfigurationContent;
        }

        public string ProvideControlsContent()
        {
            string rControlMapContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    string controlsXMLFile = Path.Combine(this.TemplatesRegistration, "Controls.xml");

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder controlMapContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(controlsXMLFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                controlMapContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rControlMapContent = controlMapContent.ToString();
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, ex);
                    }
                    catch (System.Exception)
                    {
                        rControlMapContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    DomainFileEntry fileEntry = 
                        this.Decompiler.GetFileEntry(this.TemplatesRegistration, "Controls.xml");

                    if (fileEntry.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        DomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(fileEntry.Index, fileEntry.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case DomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rControlMapContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case DomainDecompiler.RequestResults.ContentNotExists:
                                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, new FileNotFoundException());
                            case DomainDecompiler.RequestResults.PasswordError:
                                throw new System.Exception(Global.SystemMessages.PASSWORD_WRONG);
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rControlMapContent;
        }
    }
}
