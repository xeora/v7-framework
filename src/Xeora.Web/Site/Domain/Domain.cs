using System;
using System.IO;

namespace Xeora.Web.Site
{
    public class Domain : Basics.Domain.IDomain
    {
        private LanguagesHolder _LanguagesHolder;

        public Domain(string[] domainIDAccessTree) :
            this(domainIDAccessTree, null)
        { }

        public Domain(string[] domainIDAccessTree, string languageID) =>
            this.BuildDomain(domainIDAccessTree, languageID);

        private void BuildDomain(string[] domainIDAccessTree, string languageID)
        {
            if (domainIDAccessTree == null)
                domainIDAccessTree = Basics.Configurations.Xeora.Application.Main.DefaultDomain;

            try
            {
                this.Deployment = Web.Deployment.InstanceFactory.Current.GetOrCreate(domainIDAccessTree);
            }
            catch (Exception.DomainNotExistsException)
            {
                // Try with the default one if requested one is not the default one
                if (string.Compare(
                    string.Join("\\", domainIDAccessTree),
                    string.Join("\\", Basics.Configurations.Xeora.Application.Main.DefaultDomain)) != 0)
                    this.Deployment =
                        Web.Deployment.InstanceFactory.Current.GetOrCreate(
                            Basics.Configurations.Xeora.Application.Main.DefaultDomain);
                else
                    throw;
            }
            catch (System.Exception)
            {
                throw;
            }

            if (domainIDAccessTree.Length > 1)
            {
                string[] ParentDomainIDAccessTree = new string[domainIDAccessTree.Length - 1];
                Array.Copy(domainIDAccessTree, 0, ParentDomainIDAccessTree, 0, ParentDomainIDAccessTree.Length);

                this.Parent = new Domain(ParentDomainIDAccessTree);
            }

            this._LanguagesHolder = new LanguagesHolder(this, this.Deployment.Languages);
            this._LanguagesHolder.Use(languageID);
        }

        public void SetLanguageChangedListener(Action<Basics.Domain.ILanguage> languageChangedListener) =>
            this._LanguagesHolder.LanguageChangedListener = languageChangedListener;

        public string[] IDAccessTree => this.Deployment.DomainIDAccessTree;
        public Basics.Domain.Info.DeploymentTypes DeploymentType => this.Deployment.DeploymentType;
        internal Deployment.Domain Deployment { get; private set; }

        public Basics.Domain.IDomain Parent { get; private set; }
        public Basics.Domain.Info.DomainCollection Children => this.Deployment.Children;

        public string ContentsVirtualPath =>
            string.Format(
                "{0}{1}_{2}",
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                string.Join<string>("-", this.Deployment.DomainIDAccessTree),
                this._LanguagesHolder.Current.Info.ID
            );

        public Basics.Domain.ISettings Settings => this.Deployment.Settings;
        public Basics.Domain.ILanguages Languages => this._LanguagesHolder;
        internal Basics.Domain.IControls Controls => this.Deployment.Controls;
        public Basics.Domain.IxService xService => this.Deployment.xService;

        public void ProvideFileStream(string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;
            try
            {
                this.Deployment.ProvideContentFileStream(
                    this.Languages.Current.Info.ID,
                    requestedFilePath,
                    out outputStream
                );
            }
            catch (FileNotFoundException)
            {
                if (this.Parent == null)
                    throw;
            }

            if (outputStream == null && this.Parent != null)
                this.Parent.ProvideFileStream(requestedFilePath, out outputStream);
        }
    }
}