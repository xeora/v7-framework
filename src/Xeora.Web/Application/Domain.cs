using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Directives;
using Xeora.Web.Global;

namespace Xeora.Web.Application
{
    public class Domain : Basics.Domain.IDomain
    {
        private string[] _ExternalContentsUrls;
        private LanguagesHolder _LanguagesHolder;

        public Domain(string[] domainIdAccessTree, string languageId = null) =>
            this.BuildDomain(domainIdAccessTree, languageId);

        private void BuildDomain(string[] domainIdAccessTree, string languageId)
        {
            domainIdAccessTree ??= Basics.Configurations.Xeora.Application.Main.DefaultDomain;
            
            try
            {
                this.Deployment = Web.Deployment.InstanceFactory.Current.GetOrCreate(domainIdAccessTree);
            }
            catch (Exceptions.DomainNotExistsException)
            {
                // Try with the default one if requested one is not the default one
                if (string.CompareOrdinal(
                    string.Join("\\", domainIdAccessTree),
                    string.Join("\\", Basics.Configurations.Xeora.Application.Main.DefaultDomain)) != 0)
                    this.Deployment =
                        Web.Deployment.InstanceFactory.Current.GetOrCreate(
                            Basics.Configurations.Xeora.Application.Main.DefaultDomain);
                else
                    throw;
            }

            if (domainIdAccessTree.Length > 1)
            {
                string[] parentDomainIdAccessTree = new string[domainIdAccessTree.Length - 1];
                Array.Copy(domainIdAccessTree, 0, parentDomainIdAccessTree, 0, parentDomainIdAccessTree.Length);

                this.Parent = new Domain(parentDomainIdAccessTree);
            }

            this._LanguagesHolder = 
                new LanguagesHolder(this, this.Deployment.Languages);
            try
            {
                this._LanguagesHolder.Use(languageId);
            }
            catch (Exceptions.LanguageFileException)
            {
                this._LanguagesHolder.Use(this.Deployment.Settings.Configurations.DefaultLanguage);
            }

            this._ExternalContentsUrls = Basics.Configurations.Xeora.Application.Main.ExternalContentsUrls;
        }

        public void SetLanguageChangedListener(Action<Basics.Domain.ILanguage> languageChangedListener) =>
            this._LanguagesHolder.LanguageChangedListener = languageChangedListener;

        public string[] IdAccessTree => this.Deployment.DomainIdAccessTree;
        public Basics.Domain.Info.DeploymentTypes DeploymentType => this.Deployment.DeploymentType;
        private Deployment.Domain Deployment { get; set; }

        public Basics.Domain.IDomain Parent { get; private set; }
        public Basics.Domain.Info.DomainCollection Children => this.Deployment.Children;

        private long _requestCycle = 0; 
        public string ContentsVirtualPath
        {
            get
            {
                string contentsRoot =
                    Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;

                if (this._ExternalContentsUrls is { Length: > 0 })
                {
                    this._requestCycle++;
                    contentsRoot = 
                        this._ExternalContentsUrls[this._requestCycle % this._ExternalContentsUrls.Length];
                }
                
                return string.Format(
                    "{0}{1}_{2}",
                    contentsRoot,
                    string.Join<string>("-", this.Deployment.DomainIdAccessTree),
                    this._LanguagesHolder.Current.Info.Id
                );        
            }
        }

        public Basics.Domain.ISettings Settings => this.Deployment.Settings;
        public Basics.Domain.ILanguages Languages => this._LanguagesHolder;
        private Basics.Domain.IControls Controls => this.Deployment.Controls;
        public Basics.Domain.IxService xService => this.Deployment.xService;

        public void ProvideFileStream(string requestedFilePath, out Stream outputStream)
        {
            try
            {
                this.Deployment.ProvideContentFileStream(
                    this.Languages.Current.Info.Id,
                    requestedFilePath,
                    out outputStream
                );
            }
            catch (FileNotFoundException)
            {
                if (this.Parent == null)
                    throw;

                this.Parent.ProvideFileStream(requestedFilePath, out outputStream);
            }
        }

        public Basics.RenderResult Render(Basics.ServiceDefinition serviceDefinition, Basics.ControlResult.Message messageResult, string[] updateBlockControlIdStack = null) =>
            this.Render($"$T:{serviceDefinition.FullPath}$", messageResult, updateBlockControlIdStack);

        public Basics.RenderResult Render(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockControlIdStack = null)
        {
            Mother mother =
                new Mother(new Directives.Elements.Single(xeoraContent, null), messageResult, updateBlockControlIdStack);
            mother.ParseRequested += Domain.OnParseRequest;
            mother.InstanceRequested += this.OnInstanceRequest;
            mother.DeploymentAccessRequested += Domain.OnDeploymentAccessRequest;
            mother.ControlResolveRequested += Domain.OnControlResolveRequest;
            mother.Process();

            return new Basics.RenderResult(mother.Result, mother.HasInlineError);
        }

        private static void OnParseRequest(string rawValue, DirectiveCollection childrenContainer, ArgumentCollection arguments)
        {
            DateTime parseBegins = DateTime.Now;

            List<IDirective> directives =
                new List<IDirective>();

            Parser.Parse(directives.Add, rawValue, arguments);

            childrenContainer.AddRange(directives);

            if (!Basics.Configurations.Xeora.Application.Main.PrintAnalysis) return;
            
            double totalMs =
                DateTime.Now.Subtract(parseBegins).TotalMilliseconds;
            Basics.Console.Push(
                "analysed - parsed duration",
                $"{totalMs}ms - total ({directives.Count})",
                string.Empty, false, groupId: Basics.Helpers.Context.UniqueId,
                type: totalMs > Basics.Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);
        }

        private static void OnDeploymentAccessRequest(ref Basics.Domain.IDomain domain, out Deployment.Domain deployment) =>
            deployment = ((Domain)domain).Deployment;

        private void OnInstanceRequest(out Basics.Domain.IDomain domain) =>
            domain = this;
        
        private static readonly ConcurrentDictionary<string, IBase> ControlsCache =
            new ConcurrentDictionary<string, IBase>();
        private static void OnControlResolveRequest(string controlId, ref Basics.Domain.IDomain domain, out IBase control)
        {
            if (Domain.ControlsCache.TryGetValue(controlId, out IBase intactControl))
            {
                control = intactControl.Clone();
                return;
            }

            do
            {
                intactControl =
                    ((Domain)domain).Controls.Select(controlId);

                if (intactControl.Type != Basics.Domain.Control.ControlTypes.Unknown)
                {
                    Domain.ControlsCache.TryAdd(controlId, intactControl);
                    control = intactControl.Clone();

                    return;
                }

                domain = domain.Parent;
            } while (domain != null);

            control = new Controls.Unknown();
        }

        public static void Reset() =>
            Domain.ControlsCache.Clear();
    }
}