using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Xeora.Web.Deployment
{
    public class DomainDeployment : DeploymentBase, IDisposable
    {
        private string _IntegrityCheckID;
        private bool _IntegrityCheckRequires;

        private Basics.ISettings _Settings = null;
        private Basics.ILanguage _Language = null;
        private Basics.IxService _xService = null;
        private Basics.DomainInfo.DomainInfoCollection _Children = null;

        private DomainDeployment(string[] domainIDAccessTree) : base(domainIDAccessTree, null)
        { }

        public DomainDeployment(string[] domainIDAccessTree, string languageID) :
            base(domainIDAccessTree, languageID)
        {
            this._IntegrityCheckID =
                string.Format(
                    "ByPass{0}_{1}_{2}",
                    Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Replace('/', '_'),
                    string.Join<string>(":", this.DomainIDAccessTree),
                    this.DeploymentType
                );

            this._IntegrityCheckRequires = false;
            if (Basics.Helpers.Context.Application[this._IntegrityCheckID] != null)
                this._IntegrityCheckRequires = (bool)Basics.Helpers.Context.Application[this._IntegrityCheckID];

            this.Synchronise();
        }

        private void Synchronise()
        {
            if (this.DeploymentType == Basics.DomainInfo.DeploymentTypes.Development &&
                !this._IntegrityCheckRequires &&
                (
                    !Directory.Exists(this.LanguagesRegistration) ||
                    !Directory.Exists(this.TemplatesRegistration)
                )
            )
                throw new Exception.DeploymentException(string.Format("Domain {0}", Global.SystemMessages.PATH_WRONGSTRUCTURE));

            this._Settings = new Site.Setting.Settings(this.ProvideConfigurationContent());

            if (string.IsNullOrEmpty(this.LanguageID))
                base.LanguageID = this._Settings.Configurations.DefaultLanguage;

            try
            {
                this._Language = 
                    new Site.Setting.Language(
                        this.ProvideLanguageContent(this.LanguageID));
            }
            catch (System.Exception)
            {
                base.LanguageID = this._Settings.Configurations.DefaultLanguage;

                this._Language = 
                    new Site.Setting.Language(
                        this.ProvideLanguageContent(this.LanguageID));
            }

            this._xService = new Site.Setting.xService();

            // Compile Children Domains
            this._Children = 
                new Basics.DomainInfo.DomainInfoCollection();
            this.CompileChildrenDomains(ref this._Children);

            if (!this._IntegrityCheckRequires)
            {
                switch (this.DeploymentType)
                {
                    case Basics.DomainInfo.DeploymentTypes.Development:
                        this.CheckDebugIntegrity();

                        break;
                    case Basics.DomainInfo.DeploymentTypes.Release:
                        this.CheckReleaseIntegrity();

                        break;
                }
            }

            Basics.Helpers.Context.Application[this._IntegrityCheckID] = true;
        }

        private void CheckDebugIntegrity()
        {
            // Control Domain Language and Template Folders
            DirectoryInfo domainLanguagesDI = new DirectoryInfo(this.LanguagesRegistration);

            foreach (DirectoryInfo domainLanguageDI in domainLanguagesDI.GetDirectories())
            {
                if (!Directory.Exists(this.DomainContentsRegistration(domainLanguageDI.Name)))
                    throw new Exception.DeploymentException(string.Format("Domain {0}", Global.SystemMessages.PATH_WRONGSTRUCTURE));
            }
            // !--

            // -- Control Those System Essential Files are Exists! --
            string systemMessage = null;

            string controlsXML = Path.Combine(this.TemplatesRegistration, "Controls.xml");
            string configurationXML = Path.Combine(this.TemplatesRegistration, "Configuration.xml");

            if (!File.Exists(configurationXML))
            {
                systemMessage = this._Language.Get("CONFIGURATIONNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }

            if (!File.Exists(controlsXML))
            {
                systemMessage = this._Language.Get("CONTROLSXMLNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }
            // !--
        }

        private void CheckReleaseIntegrity()
        {
            // -- Control Those System Essential Files are Exists! --
            string systemMessage = null;

            XeoraDomainDecompiler.XeoraFileInfo controlsXMLFileInfo =
                this.Decompiler.GetFileInfo(this.TemplatesRegistration, "Controls.xml");
            XeoraDomainDecompiler.XeoraFileInfo configurationFileInfo =
                this.Decompiler.GetFileInfo(this.TemplatesRegistration, "Configuration.xml");

            if (configurationFileInfo.Index == -1)
            {
                systemMessage = this._Language.Get("CONFIGURATIONNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }

            if (controlsXMLFileInfo.Index == -1)
            {
                systemMessage = this._Language.Get("CONTROLSXMLNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }
            // !--
        }

        private void CompileChildrenDomains(ref Basics.DomainInfo.DomainInfoCollection childrenToFill)
        {
            DirectoryInfo childrenDI = new DirectoryInfo(this.ChildrenRootPath);

            if (childrenDI.Exists)
            {
                foreach (DirectoryInfo childDI in childrenDI.GetDirectories())
                {
                    string[] childAccessTree = 
                        new string[this.DomainIDAccessTree.Length + 1];
                    Array.Copy(this.DomainIDAccessTree, 0, childAccessTree, 0, this.DomainIDAccessTree.Length);
                    childAccessTree[childAccessTree.Length - 1] = childDI.Name;

                    DomainDeployment childDomainDeployment = 
                        InstanceFactory.Current.GetOrCreate(childAccessTree, this._Language.ID);

                    Basics.DomainInfo domainInfo =
                        new Basics.DomainInfo(
                            childDomainDeployment.DeploymentType, 
                            childDI.Name, 
                            DomainDeployment.AvailableLanguageInfos(ref childDomainDeployment)
                        );
                    domainInfo.Children.AddRange(childDomainDeployment.Children);

                    childrenToFill.Add(domainInfo);

                    childDomainDeployment.Dispose();
                }
            }
        }

        public override Basics.ISettings Settings => this._Settings;
        public override Basics.ILanguage Language => this._Language;
        public override Basics.IxService xService => this._xService;
        public override Basics.DomainInfo.DomainInfoCollection Children => this._Children;

        public override string ProvideTemplateContent(string serviceFullPath)
        {
            // -- Check is template file is exists
            if (!this.CheckTemplateExists(serviceFullPath))
            {
                string systemMessage = this._Language.Get("TEMPLATE_NOFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.TEMPLATE_NOFOUND;

                throw new Exception.DeploymentException(string.Format(systemMessage + "!", serviceFullPath));
            }
            // !--

            return base.ProvideTemplateContent(serviceFullPath);
        }

        public override void ProvideFileStream(string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;

            if (string.IsNullOrEmpty(requestedFilePath))
                return;

            requestedFilePath = requestedFilePath.Replace('/', Path.DirectorySeparatorChar);
            if (requestedFilePath[0] == Path.DirectorySeparatorChar)
                requestedFilePath = requestedFilePath.Substring(1);

            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    string requestedFileFullPath =
                        Path.Combine(this.DomainContentsRegistration(), requestedFilePath);

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
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo =
                        this.Decompiler.GetFileInfo(
                            Path.Combine(
                                this.DomainContentsRegistration(),
                                requestedFilePath.Replace(Path.GetFileName(requestedFilePath), string.Empty)
                            ),
                            Path.GetFileName(requestedFilePath)
                        );

                    if (xeoraFileInfo.Index > -1)
                    {
                        outputStream = new MemoryStream();

                        XeoraDomainDecompiler.RequestResults RequestResult =
                            this.Decompiler.ReadFile(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref outputStream);

                        if (RequestResult == XeoraDomainDecompiler.RequestResults.PasswordError)
                            throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                    }

                    break;
            }
        }

        public static Basics.DomainInfo.LanguageInfo[] AvailableLanguageInfos(string[] domainIDAccessTree)
        {
            DomainDeployment workingDomainDeployment =
                InstanceFactory.Current.GetOrCreate(domainIDAccessTree);

            return DomainDeployment.AvailableLanguageInfos(ref workingDomainDeployment);
        }

        public static Basics.DomainInfo.LanguageInfo[] AvailableLanguageInfos(ref DomainDeployment workingDomainDeployment)
        {
            List<Basics.DomainInfo.LanguageInfo> rLanguageInfos = 
                new List<Basics.DomainInfo.LanguageInfo>();

            DomainDeployment domainDeployment = null;

            switch (workingDomainDeployment.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Release:
                    Dictionary<string, XeoraDomainDecompiler.XeoraFileInfo> fileListDictionary =
                        workingDomainDeployment.Decompiler.FilesList;

                    foreach (string key in fileListDictionary.Keys)
                    {
                        if (key.IndexOf(
                            XeoraDomainDecompiler.XeoraFileInfo.CreateSearchKey(workingDomainDeployment.LanguagesRegistration, string.Empty)) == 0)
                        {
                            XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo = fileListDictionary[key];

                            domainDeployment = InstanceFactory.Current.GetOrCreate(
                                workingDomainDeployment.DomainIDAccessTree,
                                Path.GetFileNameWithoutExtension(xeoraFileInfo.FileName)
                            );

                            rLanguageInfos.Add(domainDeployment.Language.Info);

                            domainDeployment.Dispose();
                        }
                    }

                    break;
                case Basics.DomainInfo.DeploymentTypes.Development:
                    if (Directory.Exists(workingDomainDeployment.LanguagesRegistration))
                    {
                        DirectoryInfo languagesDI = new DirectoryInfo(workingDomainDeployment.LanguagesRegistration);

                        foreach (FileInfo tFI in languagesDI.GetFiles())
                        {
                            domainDeployment = InstanceFactory.Current.GetOrCreate(
                                workingDomainDeployment.DomainIDAccessTree,
                                Path.GetFileNameWithoutExtension(tFI.Name)
                            );

                            rLanguageInfos.Add(domainDeployment.Language.Info);

                            domainDeployment.Dispose();
                        }
                    }

                    break;
            }

            return rLanguageInfos.ToArray();
        }

        public static void ExtractApplication(string[] domainIDAccessTree, string extractLocation)
        {
            DomainDeployment domainDeployment =
                InstanceFactory.Current.GetOrCreate(domainIDAccessTree);
            string executablesPath = domainDeployment.ExecutablesPath;
            domainDeployment.Dispose();

            DirectoryInfo dI = new DirectoryInfo(executablesPath);

            foreach (FileInfo fI in dI.GetFiles())
            {
                if (!File.Exists(
                        Path.Combine(extractLocation, fI.Name)))
                {
                    try
                    {
                        fI.CopyTo(
                            Path.Combine(extractLocation, fI.Name));
                    }
                    catch (System.Exception)
                    {
                        // Just Handle Exceptions
                    }
                }
            }
        }

        public override void Dispose()
        {
            if (this._Settings != null)
                this._Settings.Dispose();
            if (this._Language != null)
                this._Language.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}