using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using Xeora.Web.Directives;
using Xeora.Web.Manager;

namespace Xeora.Web.Application
{
    public class DomainControl : Basics.IDomainControl
    {
        private readonly Basics.Context.IHttpContext _Context;
        private Basics.Domain.IDomain _Domain;

        private string[] _AuthenticationKeys;
        private string _ExecuteIn;
        private string _CookieSearchKeyForLanguage;

        public DomainControl(ref Basics.Context.IHttpContext context)
        {
            this._Context = context;

            this.Build();
        }

        private void Build()
        {
            this.Domain = null;

            this.ServiceDefinition = null;
            this.ServiceMimeType = string.Empty;
            this.ServiceResult = new Basics.RenderResult(string.Empty, false);

            this._ExecuteIn = string.Empty;

            this.IsAuthenticationRequired = false;
            this.IsWorkingAsStandAlone = false;

            // Check has ever user changed the Language
            this._CookieSearchKeyForLanguage =
                $"{Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Replace('/', '_')}_LanguageId";

            string languageId = string.Empty;
            Basics.Context.IHttpCookieInfo languageCookie =
                this._Context.Request.Header.Cookie[this._CookieSearchKeyForLanguage];

            if (languageCookie != null && !string.IsNullOrEmpty(languageCookie.Value))
                languageId = languageCookie.Value;
            // !---

            this.SelectDomain(languageId);

            this.MetaRecord = new MetaRecord();
        }

        public string SiteTitle { get; set; }
        public string SiteIconUrl { get; set; }
        public Basics.IMetaRecordCollection MetaRecord { get; private set; }

        public Basics.Domain.IDomain Domain
        {
            get => this._Domain;
            private set
            {
                this._Domain = value;

                if (this._Domain == null)
                    return;

                ((Domain)this.Domain).SetLanguageChangedListener((language) =>
                {
                    Basics.Context.IHttpCookieInfo languageCookie =
                        this._Context.Request.Header.Cookie[this._CookieSearchKeyForLanguage];

                    if (languageCookie == null)
                        languageCookie = this._Context.Response.Header.Cookie.CreateNewCookie(this._CookieSearchKeyForLanguage);

                    languageCookie.Value = language.Info.Id;
                    languageCookie.Expires = DateTime.Now.AddDays(30);

                    this._Context.Response.Header.Cookie.AddOrUpdate(languageCookie);
                });

                this.SiteTitle = this.Domain.Languages.Current.Get("SITETITLE");
            }
        }

        public Basics.ServiceDefinition ServiceDefinition { get; private set; }
        public Basics.Domain.ServiceTypes ServiceType { get; private set; }
        public string ServiceMimeType { get; private set; }
        public Basics.RenderResult ServiceResult { get; private set; }

        public bool IsAuthenticationRequired { get; private set; }
        public bool IsWorkingAsStandAlone { get; private set; }

        public string XeoraJSVersion => "1.0.018";

        private void SelectDomain(string languageId)
        {
            string requestedServiceId =
                this.GetRequestedServiceId(this._Context.Request.Header.Url);

            if (string.IsNullOrEmpty(requestedServiceId))
            {
                this.Domain =
                    new Domain(Basics.Configurations.Xeora.Application.Main.DefaultDomain, languageId);
                this.PrepareService(this._Context.Request.Header.Url, false);

                if (this.ServiceDefinition != null)
                    return;

                return;
            }

            // First search the request on top domains
            foreach (Basics.Domain.Info.Domain dI in this.GetAvailableDomains())
            {
                this.Domain =
                    new Domain(new [] { dI.Id }, languageId);
                this.PrepareService(this._Context.Request.Header.Url, false);

                if (this.ServiceDefinition != null)
                    return;
            }

            // If no results, start again by including children
            foreach (Basics.Domain.Info.Domain dI in this.GetAvailableDomains())
            {
                this.Domain =
                    new Domain(new [] { dI.Id }, languageId);
                this.PrepareService(this._Context.Request.Header.Url, true);

                if (this.ServiceDefinition != null)
                    return;
            }
        }

        private string GetRequestedServiceId(Basics.Context.IUrl url)
        {
            string requestFilePath =
                url.RelativePath;

            string applicationRootPath =
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            Match mR = Regex.Match(requestFilePath, $"{applicationRootPath}(\\d+/)?");
            if (mR.Success && mR.Index == 0)
                requestFilePath = requestFilePath.Remove(0, mR.Length);

            // Check if there is any query string exists! if so, template will be till there. 
            if (requestFilePath.IndexOf('?') > -1)
                requestFilePath = requestFilePath.Substring(0, requestFilePath.IndexOf('?'));

            return requestFilePath;
        }

        private void PrepareService(Basics.Context.IUrl url, bool activateChildrenSearch)
        {
            Basics.Domain.IDomain workingInstance = this.Domain;

            this.ServiceDefinition =
                this.TryResolveUrl(ref workingInstance, url);
            if (this.ServiceDefinition == null)
                return;

            Basics.Domain.IServiceItem serviceItem =
                workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ServiceDefinition.FullPath);

            if (serviceItem != null)
            {
                Basics.Domain.IDomain cachedInstance = workingInstance;
                Basics.Domain.IServiceItem cachedServiceItem = serviceItem;

                while (cachedServiceItem.Overridable)
                {
                    workingInstance = this.SearchChildrenThatOverrides(ref workingInstance, ref url);

                    // If not null, it means WorkingInstance contains a service definition which will override
                    if (workingInstance == null)
                        break;

                    cachedInstance = workingInstance;
                    cachedServiceItem =
                        workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ServiceDefinition.FullPath);

                    if (serviceItem.ServiceType != cachedServiceItem.ServiceType)
                        break;

                    // Merge or set the authenticationKeys
                    if (cachedServiceItem.Authentication)
                    {
                        if (cachedServiceItem.AuthenticationKeys.Length == 0)
                            cachedServiceItem.AuthenticationKeys = serviceItem.AuthenticationKeys;
                        else
                        {
                            // Merge
                            string[] keys = new string[cachedServiceItem.AuthenticationKeys.Length + serviceItem.AuthenticationKeys.Length];

                            Array.Copy(cachedServiceItem.AuthenticationKeys, 0, keys, 0, cachedServiceItem.AuthenticationKeys.Length);
                            Array.Copy(serviceItem.AuthenticationKeys, 0, keys, cachedServiceItem.AuthenticationKeys.Length, serviceItem.AuthenticationKeys.Length);

                            cachedServiceItem.AuthenticationKeys = keys;
                        }
                    }

                    serviceItem = cachedServiceItem;
                }

                this.ServiceType = serviceItem.ServiceType;
                this._AuthenticationKeys = serviceItem.AuthenticationKeys;

                switch (cachedServiceItem.ServiceType)
                {
                    case Basics.Domain.ServiceTypes.Template:
                        // Overrides that page does not need authentication even it has been marked as authentication required in Configuration definition
                        if (string.Compare(this.ServiceDefinition.FullPath, cachedInstance.Settings.Configurations.AuthenticationTemplate, StringComparison.OrdinalIgnoreCase) == 0)
                            this.IsAuthenticationRequired = false;
                        else
                        {
                            if (serviceItem.Authentication)
                            {
                                foreach (string authKey in serviceItem.AuthenticationKeys)
                                {
                                    if (this._Context.Session[authKey] != null) continue;
                                    
                                    this.IsAuthenticationRequired = true;
                                    break;
                                }
                            }
                        }

                        break;
                    case Basics.Domain.ServiceTypes.xService:
                        this.IsAuthenticationRequired = serviceItem.Authentication;

                        break;
                    case Basics.Domain.ServiceTypes.xSocket:
                        this.IsAuthenticationRequired = serviceItem.Authentication;

                        break;
                }

                this.IsWorkingAsStandAlone = serviceItem.StandAlone;
                this._ExecuteIn = serviceItem.ExecuteIn;
                this.ServiceMimeType = serviceItem.MimeType;

                this.Domain = cachedInstance;
            }
            else
            {
                // If ServiceItem is null but ServiceDefinition is not, then there should be a map match
                // with the a service on other domain. So start the whole process with the rewritten url

                if (this.ServiceDefinition != null && this.ServiceDefinition.Mapped)
                {
                    this.Build();

                    return;
                }

                if (!activateChildrenSearch)
                    this.ServiceDefinition = null;
                else
                {
                    // Search SubDomains For Match
                    workingInstance =
                        this.SearchChildrenThatOverrides(ref workingInstance, ref url);

                    if (workingInstance != null)
                    {
                        // Set the Working domain as child domain for this call because call requires the child domain access!
                        this.Domain = workingInstance;
                        this.PrepareService(url, true);

                        return;
                    }

                    // Nothing Found in Anywhere
                    //[Shared].Helpers.Context.Response.StatusCode = 404
                    this.ServiceDefinition = null;
                }
            }
        }

        private Basics.ServiceDefinition TryResolveUrl(ref Basics.Domain.IDomain workingInstance, Basics.Context.IUrl url)
        {
            if (workingInstance.Settings.Mappings.Active)
            {
                // First Try Dynamic Resolve
                Basics.Mapping.ResolutionResult resolutionResult =
                    this.ResolveUrl(ref workingInstance, url.RelativePath);

                if (resolutionResult == null || !resolutionResult.Resolved)
                {
                    // No Result So Check Static Definitions
                    foreach (Basics.Mapping.MappingItem mapItem in workingInstance.Settings.Mappings.Items)
                    {
                        Match rqMatch =
                            Regex.Match(url.RelativePath, mapItem.RequestMap, RegexOptions.IgnoreCase);

                        if (!rqMatch.Success) continue;
                        
                        resolutionResult = new Basics.Mapping.ResolutionResult(true, mapItem.ResolveEntry.ServiceDefinition);

                        foreach (Basics.Mapping.ResolveItem resolveItem in mapItem.ResolveEntry.ResolveItems)
                        {
                            string medItemValue = 
                                !string.IsNullOrEmpty(resolveItem.Id) 
                                    ? rqMatch.Groups[resolveItem.Id].Value 
                                    : this._Context.Request.QueryString[resolveItem.QueryStringKey];

                            resolutionResult.QueryString[resolveItem.QueryStringKey] =
                                string.IsNullOrEmpty(medItemValue) ? resolveItem.DefaultValue : medItemValue;
                        }

                        break;
                    }
                }

                if (resolutionResult != null && resolutionResult.Resolved)
                {
                    this.RectifyRequestPath(resolutionResult);

                    return resolutionResult.ServiceDefinition;
                }
            }

            // Take Care Application Path and HashCode if it is exists work with application browser path
            // this comes /APPPATH(/path?somekey=withquery)?
            // or this /APPPATH/432432/(path?somekey=withquery)?
            // or this /Standart_tr-TR/somefile.png
            // take care of it!
            string currentDomainContentPath = workingInstance.ContentsVirtualPath;

            // first test if it is domain content path
            if (url.RelativePath.IndexOf(currentDomainContentPath, StringComparison.Ordinal) == 0) return null;
            
            string requestFilePath = this.GetRequestedServiceId(url);

            if (string.IsNullOrEmpty(requestFilePath))
                return Basics.ServiceDefinition.Parse(workingInstance.Settings.Configurations.DefaultTemplate, false);
                
            Basics.ServiceDefinition rServiceDefinition =
                Basics.ServiceDefinition.Parse(requestFilePath, false);

            return string.IsNullOrEmpty(rServiceDefinition.ServiceId) ? null : rServiceDefinition;
        }

        private Basics.Domain.IDomain SearchChildrenThatOverrides(ref Basics.Domain.IDomain workingInstance, ref Basics.Context.IUrl url)
        {
            if (workingInstance == null)
                return null;

            List<string> childDomainIdAccessTree = new List<string>();
            childDomainIdAccessTree.AddRange(workingInstance.IdAccessTree);

            foreach (Basics.Domain.Info.Domain childDI in workingInstance.Children)
            {
                childDomainIdAccessTree.Add(childDI.Id);

                Basics.Domain.IDomain rDomainInstance =
                    new Domain(childDomainIdAccessTree.ToArray(), this.Domain.Languages.Current.Info.Id);

                Basics.ServiceDefinition serviceDefinition =
                    this.TryResolveUrl(ref rDomainInstance, url);
                if (serviceDefinition == null)
                {
                    childDomainIdAccessTree.RemoveAt(childDomainIdAccessTree.Count - 1);

                    continue;
                }

                if (rDomainInstance.Settings.Services.ServiceItems.GetServiceItem(serviceDefinition.FullPath) == null)
                {
                    if (rDomainInstance.Children.Count > 0)
                    {
                        rDomainInstance = this.SearchChildrenThatOverrides(ref rDomainInstance, ref url);

                        if (rDomainInstance != null)
                            return rDomainInstance;
                    }

                    if (serviceDefinition.Mapped)
                    {
                        url = this._Context.Request.Header.Url;

                        return workingInstance;
                    }
                }
                else
                    return rDomainInstance;

                childDomainIdAccessTree.RemoveAt(childDomainIdAccessTree.Count - 1);
            }

            return null;
        }

        private void RectifyRequestPath(Basics.Mapping.ResolutionResult resolutionResult)
        {
            string requestUrl = 
                string.Format(
                    "{0}{1}",
                    Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                    resolutionResult.ServiceDefinition.FullPath
                );
            if (resolutionResult.QueryString.Count > 0)
                requestUrl = string.Concat(requestUrl, "?", resolutionResult.QueryString.ToString());

            // Let the server understand what this Url is about...
            this._Context.Request.RewritePath(requestUrl);
        }

        public void OverrideDomain(string[] domainIdAccessTree, string languageId) =>
            this.Domain = new Domain(domainIdAccessTree, languageId);

        public void ProvideXeoraJsStream(ref Stream outputStream)
        {
            if (outputStream == null)
                throw new NullReferenceException();
            
            outputStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"Xeora.Web._sps_v{this.XeoraJSVersion}.js");
        }

        public Basics.Execution.Bind GetxSocketBind()
        {
            Basics.Execution.Bind rBind = null;

            if (this.ServiceType == Basics.Domain.ServiceTypes.xSocket &&
                !string.IsNullOrEmpty(this._ExecuteIn))
                rBind = Basics.Execution.Bind.Make(this._ExecuteIn);

            return rBind;
        }

        public void RenderService(Basics.ControlResult.Message messageResult, string[] updateBlockControlIdStack)
        {
            if (this.ServiceDefinition == null)
                throw new Exception(Global.SystemMessages.TEMPLATE_IDMUSTBESET + "!");

            switch (this.ServiceType)
            {
                case Basics.Domain.ServiceTypes.Template:
                    this.ServiceResult = 
                        this.Domain.Render(this.ServiceDefinition, messageResult, updateBlockControlIdStack);

                    break;
                case Basics.Domain.ServiceTypes.xService:
                    if (this.IsAuthenticationRequired)
                    {
                        Basics.X.ServiceParameterCollection serviceParameterCol =
                            new Basics.X.ServiceParameterCollection();
                        serviceParameterCol.ParseXml(this._Context.Request.Body.Form["xParams"]);

                        if (serviceParameterCol.PublicKey != null)
                        {
                            this.IsAuthenticationRequired = false;

                            foreach (string authKey in this._AuthenticationKeys)
                            {
                                if (this.Domain.xService.ReadSessionVariable(serviceParameterCol.PublicKey, authKey) != null) continue;
                                
                                this.IsAuthenticationRequired = true;
                                break;
                            }
                        }
                    }

                    if (!this.IsAuthenticationRequired)
                        this.ServiceResult = this.Domain.xService.Render(this._ExecuteIn, this.ServiceDefinition.ServiceId);
                    else
                    {
                        object methodResult = new SecurityException(Global.SystemMessages.XSERVICE_AUTH);

                        this.ServiceResult = this.Domain.xService.GenerateXml(methodResult);
                    }

                    break;
            }
        }

        public Basics.Mapping.ResolutionResult ResolveUrl(string requestFilePath)
        {
            Basics.Domain.IDomain dummy = null;
            return this.ResolveUrl(ref dummy, requestFilePath);
        }

        private Basics.Mapping.ResolutionResult ResolveUrl(ref Basics.Domain.IDomain workingInstance, string requestFilePath)
        {
            if (workingInstance == null)
                workingInstance = this.Domain;

            if (workingInstance != null &&
                workingInstance.Settings.Mappings.Active &&
                !string.IsNullOrEmpty(workingInstance.Settings.Mappings.ResolverExecutable))
            {
                Basics.Execution.Bind resolverBind =
                    Basics.Execution.Bind.Make($"{workingInstance.Settings.Mappings.ResolverExecutable}?ResolveURL,rfp");
                resolverBind.Parameters.Prepare(parameter => requestFilePath);
                resolverBind.InstanceExecution = true;

                Basics.Execution.InvokeResult<Basics.Mapping.ResolutionResult> resolverInvokeResult =
                    AssemblyCore.InvokeBind<Basics.Mapping.ResolutionResult>(Basics.Helpers.Context.Request.Header.Method, resolverBind, ExecuterTypes.Undefined);

                if (resolverInvokeResult.Exception == null)
                    return resolverInvokeResult.Result;
            }

            return null;
        }

        // Cache for performance consideration
        private static Basics.Domain.Info.DomainCollection _AvailableDomains;
        public Basics.Domain.Info.DomainCollection GetAvailableDomains()
        {
            DirectoryInfo domainDI = new DirectoryInfo(
                Path.GetFullPath(
                    Path.Combine(
                        Basics.Configurations.Xeora.Application.Main.PhysicalRoot,
                        Basics.Configurations.Xeora.Application.Main.ApplicationRoot.FileSystemImplementation,
                        "Domains"
                    )
                )
            );

            if (DomainControl._AvailableDomains != null)
            {
                if (DomainControl._AvailableDomains.Count != domainDI.GetDirectories().Length)
                {
                    DomainControl._AvailableDomains = null;
                    return this.GetAvailableDomains();
                }

                foreach (DirectoryInfo dI in domainDI.GetDirectories())
                {
                    if (DomainControl._AvailableDomains[dI.Name] == null)
                    {
                        DomainControl._AvailableDomains = null;
                        return this.GetAvailableDomains();
                    }
                }

                return DomainControl._AvailableDomains;
            }

            Basics.Domain.Info.DomainCollection rDomainInfoCollection =
                new Basics.Domain.Info.DomainCollection();

            foreach (DirectoryInfo dI in domainDI.GetDirectories())
            {
                try
                {
                    Deployment.Domain deployment =
                        Deployment.InstanceFactory.Current.GetOrCreate(new [] { dI.Name });

                    List<Basics.Domain.Info.Language> languages =
                        new List<Basics.Domain.Info.Language>();

                    foreach (string languageId in deployment.Languages)
                        languages.Add(deployment.Languages[languageId].Info);

                    Basics.Domain.Info.Domain domainInfo =
                        new Basics.Domain.Info.Domain(deployment.DeploymentType, dI.Name, languages.ToArray());
                    domainInfo.Children.AddRange(deployment.Children);

                    rDomainInfoCollection.Add(domainInfo);
                }
                catch (Exception ex)
                {
                    Tools.EventLogger.Log(ex);
                }
            }
            DomainControl._AvailableDomains = rDomainInfoCollection;

            return DomainControl._AvailableDomains;
        }

        public void ClearCache()
        {
            // Clear Compiled Statements Cache
            Master.ClearCache();

            // Clear Domain Control Map Cache
            this.Domain.ClearCache();

            // Clear Deployment Template and File Cache
            Deployment.InstanceFactory.Current.Reset();

            // Clear Partial Block Cache
            PartialCachePool.Current.Reset(this.Domain.IdAccessTree);
        }
    }
}
