using System;
using System.Collections.Generic;
using System.IO;

namespace Xeora.Web.Deployment
{
    public class Domain : IDisposable
    {
        private Basics.Domain.Info.DomainCollection _Children;

        public Domain(string[] domainIdAccessTree)
        {
            this.DomainIdAccessTree = domainIdAccessTree;

            if (this.DomainIdAccessTree == null ||
                this.DomainIdAccessTree.Length == 0)
                throw new Exceptions.DeploymentException(Global.SystemMessages.IDMUSTBESET);

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
                throw new Exceptions.DomainNotExistsException(
                    Global.SystemMessages.PATH_NOTEXISTS,
                    new DirectoryNotFoundException($"DomainRootPath: {domainRootPath}")
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

        public string[] DomainIdAccessTree { get; }
        public Basics.Domain.Info.DeploymentTypes DeploymentType { get; }
        private IDeployment Deployment { get; set; }

        private string CreateDomainAccessPathString()
        {
            string rDomainAccessPath = this.DomainIdAccessTree[0];

            for (int iC = 1; iC < this.DomainIdAccessTree.Length; iC++)
                rDomainAccessPath = Path.Combine(rDomainAccessPath, "Addons", this.DomainIdAccessTree[iC]);

            return rDomainAccessPath;
        }

        private void LoadDomain()
        {
            this.Settings =
                new Application.Configurations.Settings(this.Deployment.ProvideConfigurationContent());

            // Setup Languages
            string[] languageIds = this.Deployment.Languages;
            if (languageIds.Length == 0)
                throw new Exceptions.LanguageFileException();

            this.Languages = new Application.Configurations.Languages();

            foreach (string languageId in languageIds)
            {
                ((Application.Configurations.Languages)this.Languages).Add(
                    new Application.Configurations.Language(
                        this.Deployment.ProvideLanguageContent(languageId),
                        string.CompareOrdinal(languageId, this.Settings.Configurations.DefaultLanguage) == 0
                    )
                );
            }
            // !---

            this.Controls =
                new Application.ControlManager(this.Deployment.ProvideControlsContent());
            this.xService = new Application.Configurations.xService();

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
                    new string[this.DomainIdAccessTree.Length + 1];
                Array.Copy(this.DomainIdAccessTree, 0, childAccessTree, 0, this.DomainIdAccessTree.Length);
                childAccessTree[^1] = childDI.Name;

                Domain childDomain = 
                    new Domain(childAccessTree);

                List<Basics.Domain.Info.Language> languages =
                    new List<Basics.Domain.Info.Language>();

                foreach (string languageId in childDomain.Languages)
                    languages.Add(childDomain.Languages[languageId].Info);

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

        public Basics.Domain.ISettings Settings { get; private set; }
        public Basics.Domain.ILanguages Languages { get; private set; }
        public Basics.Domain.IControls Controls { get; private set; }
        public Basics.Domain.IxService xService { get; private set; }
        public Basics.Domain.Info.DomainCollection Children => this._Children;

        public void ProvideContentFileStream(string languageId, string requestedFilePath, out Stream outputStream) =>
            this.Deployment.ProvideContentFileStream(languageId, requestedFilePath, out outputStream);

        public string ProvideTemplateContent(string serviceFullPath) => 
            this.Deployment.ProvideTemplateContent(serviceFullPath);

        public static void ExtractApplication(string[] domainIdAccessTree, string extractLocation)
        {
            Domain domain =
                InstanceFactory.Current.GetOrCreate(domainIdAccessTree);
            string executablesPath = domain.Deployment.ExecutablesRegistration;
            domain.Dispose();

            DirectoryInfo dI = 
                new DirectoryInfo(executablesPath);

            foreach (FileInfo fI in dI.GetFiles())
            {
                if (File.Exists(
                    Path.Combine(extractLocation, fI.Name))) continue;
                
                try
                {
                    fI.CopyTo(
                        Path.Combine(extractLocation, fI.Name));
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
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
            Settings?.Dispose();
            Languages?.Dispose();
            Controls?.Dispose();
        }
    }
}