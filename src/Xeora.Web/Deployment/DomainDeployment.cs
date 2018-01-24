using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Xeora.Web.Deployment
{
    public class DomainDeployment : DeploymentBase, IDisposable
    {
        private static ConcurrentDictionary<string, bool> _IntegrityCheckResults =
            new ConcurrentDictionary<string, bool>();

        private Basics.Domain.ISettings _Settings = null;
        private Basics.Domain.ILanguages _Languages = null;
        private Basics.Domain.IControls _Controls = null;
        private Basics.Domain.IxService _xService = null;
        private Basics.Domain.Info.DomainCollection _Children = null;

        public DomainDeployment(string[] domainIDAccessTree)
            : base(domainIDAccessTree)
        {
            string integrityCheckID =
                string.Format(
                    "ByPass{0}_{1}_{2}",
                    Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Replace('/', '_'),
                    string.Join<string>(":", this.DomainIDAccessTree),
                    this.DeploymentType
                );

            bool integrityCheckRequires;
            if (!DomainDeployment._IntegrityCheckResults.TryGetValue(integrityCheckID, out integrityCheckRequires))
                integrityCheckRequires = true;

            this.Synchronise(integrityCheckRequires);
            if (integrityCheckRequires)
                this.IntegrityCheck();

            DomainDeployment._IntegrityCheckResults.TryAdd(integrityCheckID, false);
        }

        private void Synchronise(bool integrityCheckRequires)
        {
            if (this.DeploymentType == Basics.Domain.Info.DeploymentTypes.Development &&
                !integrityCheckRequires &&
                (
                    !Directory.Exists(this.LanguagesRegistration) ||
                    !Directory.Exists(this.TemplatesRegistration)
                )
            )
                throw new Exception.DeploymentException(string.Format("Domain {0}", Global.SystemMessages.PATH_WRONGSTRUCTURE));

            this._Settings = new Site.Setting.Settings(this.ProvideConfigurationContent());

            // Setup Languages
            string[] languageIDs = this.GetLanguageIDs();
            if (languageIDs.Length == 0)
                throw new Exception.LanguageFileException();

            this._Languages = new Site.Setting.Languages();

            foreach (string languageID in languageIDs)
            {
                ((Site.Setting.Languages)this._Languages).Add(
                    new Site.Setting.Language(
                        this.ProvideLanguageContent(languageID),
                        string.Compare(languageID, this._Settings.Configurations.DefaultLanguage) == 0
                    )
                );
            }
            // !---

            this._Controls = new Site.Setting.Controls(this.ProvideControlsContent());
            this._xService = new Site.Setting.xService();

            // Compile Children Domains
            this._Children =
                new Basics.Domain.Info.DomainCollection();
            this.CompileChildrenDomains(ref this._Children);
        }

        private void IntegrityCheck()
        {
            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    this.CheckDebugIntegrity();

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    this.CheckReleaseIntegrity();

                    break;
            }
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
                systemMessage = this._Languages.Current.Get("CONFIGURATIONNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }

            if (!File.Exists(controlsXML))
            {
                systemMessage = this._Languages.Current.Get("CONTROLSXMLNOTFOUND");

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

            DomainFileEntry controlsXMLFileEntry =
                this.Decompiler.GetFileEntry(this.TemplatesRegistration, "Controls.xml");
            DomainFileEntry configurationFileEntry =
                this.Decompiler.GetFileEntry(this.TemplatesRegistration, "Configuration.xml");

            if (configurationFileEntry.Index == -1)
            {
                systemMessage = this._Languages.Current.Get("CONFIGURATIONNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }

            if (controlsXMLFileEntry.Index == -1)
            {
                systemMessage = this._Languages.Current.Get("CONTROLSXMLNOTFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND;

                throw new Exception.DeploymentException(systemMessage + "!");
            }
            // !--
        }

        private void CompileChildrenDomains(ref Basics.Domain.Info.DomainCollection childrenToFill)
        {
            DirectoryInfo childrenDI = 
                new DirectoryInfo(this.ChildrenRootPath);

            if (childrenDI.Exists)
            {
                foreach (DirectoryInfo childDI in childrenDI.GetDirectories())
                {
                    string[] childAccessTree =
                        new string[this.DomainIDAccessTree.Length + 1];
                    Array.Copy(this.DomainIDAccessTree, 0, childAccessTree, 0, this.DomainIDAccessTree.Length);
                    childAccessTree[childAccessTree.Length - 1] = childDI.Name;

                    DomainDeployment childDomainDeployment = 
                        new DomainDeployment(childAccessTree);

                    List<Basics.Domain.Info.Language> languages = 
                        new List<Basics.Domain.Info.Language>();

                    foreach (string languageID in childDomainDeployment.Languages)
                        languages.Add(childDomainDeployment.Languages[languageID].Info);

                    Basics.Domain.Info.Domain domainInfo =
                        new Basics.Domain.Info.Domain(
                            childDomainDeployment.DeploymentType,
                            childDI.Name,
                            languages.ToArray()
                        );
                    domainInfo.Children.AddRange(childDomainDeployment.Children);

                    childrenToFill.Add(domainInfo);

                    childDomainDeployment.Dispose();
                }
            }
        }

        public override Basics.Domain.ISettings Settings => this._Settings;
        public override Basics.Domain.ILanguages Languages => this._Languages;
        public override Basics.Domain.IControls Controls => this._Controls;
        public override Basics.Domain.IxService xService => this._xService;
        public override Basics.Domain.Info.DomainCollection Children => this._Children;

        public override string ProvideTemplateContent(string serviceFullPath)
        {
            // -- Check is template file is exists
            if (!this.CheckTemplateExists(serviceFullPath))
            {
                string systemMessage = this._Languages.Current.Get("TEMPLATE_NOFOUND");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.TEMPLATE_NOFOUND;

                throw new Exception.DeploymentException(string.Format(systemMessage + "!", serviceFullPath));
            }
            // !--

            return base.ProvideTemplateContent(serviceFullPath);
        }

        public string[] GetLanguageIDs()
        {
            List<string> rLanguageIDs =
                new List<string>();

            switch (this.DeploymentType)
            {
                case Basics.Domain.Info.DeploymentTypes.Development:
                    if (Directory.Exists(this.LanguagesRegistration))
                    {
                        DirectoryInfo languagesDI = new DirectoryInfo(this.LanguagesRegistration);

                        foreach (FileInfo tFI in languagesDI.GetFiles())
                            rLanguageIDs.Add(Path.GetFileNameWithoutExtension(tFI.Name));
                    }

                    break;
                case Basics.Domain.Info.DeploymentTypes.Release:
                    Dictionary<string, DomainFileEntry> fileEntryDictionary =
                        this.Decompiler.FilesList;

                    foreach (string key in fileEntryDictionary.Keys)
                    {
                        if (key.IndexOf(
                                DomainFileEntry.CreateSearchKey(this.LanguagesRegistration, string.Empty)) == 0)
                        {
                            DomainFileEntry fileEntry = fileEntryDictionary[key];

                            rLanguageIDs.Add(Path.GetFileNameWithoutExtension(fileEntry.FileName));
                        }
                    }

                    break;
            }

            return rLanguageIDs.ToArray();
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
            if (this._Languages != null)
                this._Languages.Dispose();
            if (this._Controls != null)
                this._Controls.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}