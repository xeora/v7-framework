using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Directives;
using Xeora.Web.Global;

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

        public Basics.RenderResult Render(Basics.ServiceDefinition serviceDefinition, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null) =>
            this.Render(string.Format("$T:{0}$", serviceDefinition.FullPath), messageResult, updateBlockControlIDStack);

        public Basics.RenderResult Render(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null)
        {
            Mother mother =
                new Mother(xeoraContent, messageResult, updateBlockControlIDStack);
            mother.ParseRequested += this.OnParseRequest;
            mother.InstanceRequested += this.OnInstanceRequest;
            mother.DeploymentAccessRequested += this.OnDeploymentAccessRequest;
            mother.ControlResolveRequested += this.OnControlResolveRequest;
            mother.Process();

            return new Basics.RenderResult(mother.Result, mother.HasInlineError);
        }

        private void OnParseRequest(string rawValue, ref DirectiveCollection childrenContainer, ArgumentCollection arguments)
        {
            DateTime parseBegins = DateTime.Now;

            List<IDirective> directives =
                new List<IDirective>();

            Parser.Parse(directives.Add, rawValue, arguments);

            childrenContainer.AddRange(directives);

            if (Basics.Configurations.Xeora.Application.Main.PrintAnalytics)
            {
                Basics.Console.Push(
                    "analytic - parsed duration",
                    string.Format("{0}ms - total ({1})", DateTime.Now.Subtract(parseBegins).TotalMilliseconds, directives.Count),
                    string.Empty, false);
            }
        }

        private void OnDeploymentAccessRequest(ref Basics.Domain.IDomain domain, ref Deployment.Domain deployment) =>
            deployment = ((Domain)domain).Deployment;

        private void OnInstanceRequest(ref Basics.Domain.IDomain domain) =>
            domain = this;

        private static readonly ConcurrentDictionary<string, IBase> _ControlsCache =
            new ConcurrentDictionary<string, IBase>();
        private void OnControlResolveRequest(string controlID, ref Basics.Domain.IDomain domain, out IBase control)
        {
            if (Domain._ControlsCache.TryGetValue(controlID, out IBase intactControl))
            {
                control = intactControl.Clone();
                return;
            }

            do
            {
                intactControl =
                    ((Domain)domain).Controls.Select(controlID);

                if (intactControl.Type != Basics.Domain.Control.ControlTypes.Unknown)
                {
                    Domain._ControlsCache.TryAdd(controlID, intactControl);
                    control = intactControl.Clone();

                    return;
                }

                domain = domain.Parent;
            } while (domain != null);

            control = new Setting.Control.Unknown();
        }

        public void ClearCache() =>
            Domain._ControlsCache.Clear();
    }
}