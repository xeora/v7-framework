using System;
using System.Collections.Generic;
using System.IO;

namespace Xeora.Web.Deployment
{
    public class Domain : IDisposable
    {
        private Basics.Domain.ISettings _Settings = null;
        private Basics.Domain.ILanguages _Languages = null;
        private Basics.Domain.IControls _Controls = null;
        private Basics.Domain.IxService _xService = null;
        private Basics.Domain.Info.DomainCollection _Children = null;

        public Domain(string[] domainIDAccessTree)
        {
            this.DomainIDAccessTree = domainIDAccessTree;

            if (this.DomainIDAccessTree == null ||
                this.DomainIDAccessTree.Length == 0)
                throw new Exception.DeploymentException(Global.SystemMessages.IDMUSTBESET);

            string domainRootPath =
                Path.GetFullPath(
                    Path.Combine(
                        Basics.Configurations.Xeora.Application.Main.PhysicalRoot,
                        Basics.Configurations.Xeora.Application.Main.ApplicationRoot.FileSystemImplementation,
                        "Domains",
                        this.CreateDomainAccessPathString()
                    )
                );

            if (!Directory.Exists(domainRootPath))
                throw new Exception.DomainNotExistsException(
                    Global.SystemMessages.PATH_NOTEXISTS,
                    new DirectoryNotFoundException(string.Format("DomainRootPath: {0}", domainRootPath))
                );

            string releaseTestPath =
                Path.Combine(domainRootPath, "Content.xeora");

            if (File.Exists(releaseTestPath))
            {
                this.Deployment = new Release(domainRootPath);
                this.DeploymentType = Basics.Domain.Info.DeploymentTypes.Release;
            }
            else
            {
                this.Deployment = new Development(domainRootPath);
                this.DeploymentType = Basics.Domain.Info.DeploymentTypes.Development;
            }

            this.LoadDomain();
        }

        public string[] DomainIDAccessTree { get; private set; }
        public Basics.Domain.Info.DeploymentTypes DeploymentType { get; private set; }
        private IDeployment Deployment { get; set; }

        private string CreateDomainAccessPathString()
        {
            string rDomainAccessPath = this.DomainIDAccessTree[0];

            for (int iC = 1; iC < this.DomainIDAccessTree.Length; iC++)
                rDomainAccessPath = Path.Combine(rDomainAccessPath, "Addons", this.DomainIDAccessTree[iC]);

            return rDomainAccessPath;
        }

        private void LoadDomain()
        {
            this._Settings =
                new Site.Setting.Settings(this.Deployment.ProvideConfigurationContent());

            // Setup Languages
            string[] languageIDs = this.Deployment.Languages;
            if (languageIDs.Length == 0)
                throw new Exception.LanguageFileException();

            this._Languages = new Site.Setting.Languages();

            foreach (string languageID in languageIDs)
            {
                ((Site.Setting.Languages)this._Languages).Add(
                    new Site.Setting.Language(
                        this.Deployment.ProvideLanguageContent(languageID),
                        string.Compare(languageID, this._Settings.Configurations.DefaultLanguage) == 0
                    )
                );
            }
            // !---

            this._Controls =
                new Site.Setting.Controls(this.Deployment.ProvideControlsContent());
            this._xService = new Site.Setting.xService();

            // Compile Children Domains
            this._Children =
                new Basics.Domain.Info.DomainCollection();
            this.CompileChildrenDomains(ref this._Children);
        }

        private void CompileChildrenDomains(ref Basics.Domain.Info.DomainCollection childrenToFill)
        {
            DirectoryInfo childrenDI =
                new DirectoryInfo(this.Deployment.ChildrenRegistration);

            if (!childrenDI.Exists)
                return;

            foreach (DirectoryInfo childDI in childrenDI.GetDirectories())
            {
                string[] childAccessTree =
                    new string[this.DomainIDAccessTree.Length + 1];
                Array.Copy(this.DomainIDAccessTree, 0, childAccessTree, 0, this.DomainIDAccessTree.Length);
                childAccessTree[childAccessTree.Length - 1] = childDI.Name;

                Domain childDomain = 
                    new Domain(childAccessTree);

                List<Basics.Domain.Info.Language> languages =
                    new List<Basics.Domain.Info.Language>();

                foreach (string languageID in childDomain.Languages)
                    languages.Add(childDomain.Languages[languageID].Info);

                Basics.Domain.Info.Domain domainInfo =
                    new Basics.Domain.Info.Domain(
                        childDomain.DeploymentType,
                        childDI.Name,
                        languages.ToArray()
                    );
                domainInfo.Children.AddRange(childDomain.Children);

                childrenToFill.Add(domainInfo);

                childDomain.Dispose();
            }
        }

        public Basics.Domain.ISettings Settings => this._Settings;
        public Basics.Domain.ILanguages Languages => this._Languages;
        public Basics.Domain.IControls Controls => this._Controls;
        public Basics.Domain.IxService xService => this._xService;
        public Basics.Domain.Info.DomainCollection Children => this._Children;

        public void ProvideContentFileStream(string languageID, string requestedFilePath, out Stream outputStream) =>
            this.Deployment.ProvideContentFileStream(languageID, requestedFilePath, out outputStream);

        public string ProvideTemplateContent(string serviceFullPath) => 
            this.Deployment.ProvideTemplateContent(serviceFullPath);

        public static void ExtractApplication(string[] domainIDAccessTree, string extractLocation)
        {
            Domain domain =
                InstanceFactory.Current.GetOrCreate(domainIDAccessTree);
            string executablesPath = domain.Deployment.ExecutablesRegistration;
            domain.Dispose();

            DirectoryInfo dI = 
                new DirectoryInfo(executablesPath);

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

        public void Reload()
        {
            if (this.Deployment.Reload())
                this.LoadDomain();
        }

        public void Dispose()
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