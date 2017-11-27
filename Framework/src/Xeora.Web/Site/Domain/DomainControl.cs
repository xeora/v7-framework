using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;

namespace Xeora.Web.Site
{
    public class DomainControl : Basics.IDomainControl, IDisposable
    {
        private Basics.Context.IHttpContext _Context;

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

            this.ServicePathInfo = null;
            this.ServiceMimeType = string.Empty;
            this.ServiceResult = string.Empty;

            this._ExecuteIn = string.Empty;

            this.IsAuthenticationRequired = false;
            this.IsWorkingAsStandAlone = false;

            // Check has ever user changed the Language
            this._CookieSearchKeyForLanguage =
                string.Format(
                    "{0}_LanguageID",
                    Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation.Replace('/', '_'));

            string languageID = string.Empty;
            Basics.Context.IHttpCookieInfo languageCookie =
                this._Context.Request.Header.Cookie[this._CookieSearchKeyForLanguage];

            if (languageCookie != null && !string.IsNullOrEmpty(languageCookie.Value))
                languageID = languageCookie.Value;
            // !---

            this.SelectDomain(languageID);

            if (this.Domain != null)
                this.SiteTitle = this.Domain.Language.Get("SITETITLE");

            this.MetaRecord = new MetaRecord();
        }

        public string SiteTitle { get; set; }
        public string SiteIconURL { get; set; }
        public Basics.IMetaRecordCollection MetaRecord { get; private set; }

        public Basics.IDomain Domain { get; private set; }

        public Basics.ServicePathInfo ServicePathInfo { get; private set; }
        public Basics.ServiceTypes ServiceType { get; private set; }
        public string ServiceMimeType { get; private set; }
        public string ServiceResult { get; private set; }

        public bool IsAuthenticationRequired { get; private set; }
        public bool IsWorkingAsStandAlone { get; private set; }

        public string XeoraJSVersion => "1.0.017";

        private void SelectDomain(string languageID)
        {
            string requestedServiceID =
                this.GetRequestedServiceID(this._Context.Request.Header.URL);

            if (string.IsNullOrEmpty(requestedServiceID))
            {
                this.Domain =
                    new Domain(Basics.Configurations.Xeora.Application.Main.DefaultDomain, languageID);
                this.PrepareService(this._Context.Request.Header.URL, false);

                if (this.ServicePathInfo != null)
                    return;

                this.Domain.Dispose();

                return;
            }

            // First search the request on top domains
            foreach (Basics.DomainInfo dI in this.GetAvailableDomains())
            {
                this.Domain =
                    new Domain(new string[] { dI.ID }, languageID);
                this.PrepareService(this._Context.Request.Header.URL, false);

                if (this.ServicePathInfo != null)
                    return;

                this.Domain.Dispose();
            }

            // If no results, start again by including children
            foreach (Basics.DomainInfo dI in this.GetAvailableDomains())
            {
                this.Domain =
                    new Domain(new string[] { dI.ID }, languageID);
                this.PrepareService(this._Context.Request.Header.URL, true);

                if (this.ServicePathInfo != null)
                    return;

                this.Domain.Dispose();
            }
        }

        private string GetRequestedServiceID(Basics.Context.IURL url)
        {
            string requestFilePath =
                url.RelativePath;

            string applicationRootPath =
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            Match mR = Regex.Match(requestFilePath, string.Format("{0}(\\d+/)?", applicationRootPath));
            if (mR.Success && mR.Index == 0)
                requestFilePath = requestFilePath.Remove(0, mR.Length);

            // Check if there is any query string exists! if so, template will be till there. 
            if (requestFilePath.IndexOf('?') > -1)
                requestFilePath = requestFilePath.Substring(0, requestFilePath.IndexOf('?'));

            return requestFilePath;
        }

        private void PrepareService(Basics.Context.IURL url, bool activateChildrenSearch)
        {
            Basics.IDomain workingInstance = this.Domain;

            this.ServicePathInfo =
                this.TryResolveURL(ref workingInstance, url);
            if (this.ServicePathInfo == null)
                return;

            Basics.IServiceItem serviceItem =
                workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ServicePathInfo.FullPath);

            if (serviceItem != null)
            {
                Basics.IDomain cachedInstance = workingInstance;
                Basics.IServiceItem cachedServiceItem = serviceItem;

                while (cachedServiceItem.Overridable)
                {
                    workingInstance = this.SearchChildrenThatOverrides(ref workingInstance, ref url);

                    // If not null, it means WorkingInstance contains a service definition which will override
                    if (workingInstance == null)
                        break;

                    cachedInstance = workingInstance;
                    cachedServiceItem =
                        workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ServicePathInfo.FullPath);

                    if (serviceItem.ServiceType != cachedServiceItem.ServiceType)
                        break;

                    // Merge or set the authenticationkeys
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
                    case Basics.ServiceTypes.Template:
                        // Overrides that page does not need authentication even it has been marked as authentication required in Configuration definition
                        if (string.Compare(this.ServicePathInfo.FullPath, cachedInstance.Settings.Configurations.AuthenticationPage, true) == 0)
                            this.IsAuthenticationRequired = false;
                        else
                        {
                            if (serviceItem.Authentication)
                            {
                                foreach (string authKey in serviceItem.AuthenticationKeys)
                                {
                                    if (this._Context.Session[authKey] == null)
                                    {
                                        this.IsAuthenticationRequired = true;

                                        break;
                                    }
                                }
                            }
                        }

                        break;
                    case Basics.ServiceTypes.xService:
                        this.IsAuthenticationRequired = serviceItem.Authentication;

                        break;
                    case Basics.ServiceTypes.xSocket:
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
                // If ServiceItem is null but ServicePathInfo is not, then there should be a map match
                // with the a service on other domain. So start the whole process with the rewritten url

                if (this.ServicePathInfo != null && this.ServicePathInfo.IsMapped)
                {
                    this.Build();

                    return;
                }

                if (!activateChildrenSearch)
                    this.ServicePathInfo = null;
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
                    this.ServicePathInfo = null;
                }
            }
        }

        private Basics.ServicePathInfo TryResolveURL(ref Basics.IDomain workingInstance, Basics.Context.IURL url)
        {
            if (workingInstance.Settings.URLMappings.IsActive)
            {
                // First Try Dynamic Resolve
                Basics.URLMapping.ResolvedMapped resolvedMapped =
                    this.QueryURLResolver(ref workingInstance, url.RelativePath);

                if (resolvedMapped == null || !resolvedMapped.IsResolved)
                {
                    // No Result So Check Static Definitions
                    foreach (Basics.URLMapping.URLMappingItem urlMapItem in workingInstance.Settings.URLMappings.Items)
                    {
                        Match rqMatch =
                            Regex.Match(url.RelativePath, urlMapItem.RequestMap, RegexOptions.IgnoreCase);

                        if (rqMatch.Success)
                        {
                            resolvedMapped = new Basics.URLMapping.ResolvedMapped(true, urlMapItem.ResolveInfo.ServicePathInfo);

                            string medItemValue = null;
                            foreach (Basics.URLMapping.ResolveInfos.MappedItem medItem in urlMapItem.ResolveInfo.MappedItems)
                            {
                                medItemValue = string.Empty;

                                if (!string.IsNullOrEmpty(medItem.ID))
                                    medItemValue = rqMatch.Groups[medItem.ID].Value;
                                else
                                    medItemValue = this._Context.Request.QueryString[medItem.QueryStringKey];

                                resolvedMapped.URLQueryDictionary[medItem.QueryStringKey] =
                                    (string.IsNullOrEmpty(medItemValue) ? medItem.DefaultValue : medItemValue);
                            }

                            break;
                        }
                    }
                }

                if (resolvedMapped != null && resolvedMapped.IsResolved)
                {
                    this.RectifyRequestPath(resolvedMapped);

                    return resolvedMapped.ServicePathInfo;
                }
            }

            // Take Care Application Path and HashCode if it is exists work with application browser path
            // this comes /APPPATH(/path?somekey=withquery)?
            // or this /APPPATH/432432/(path?somekey=withquery)?
            // or this /Standart_tr-TR/somefile.png
            // take care of it!
            string currentDomainContentPath = workingInstance.ContentsVirtualPath;

            // first test if it is domain content path
            if (url.RelativePath.IndexOf(currentDomainContentPath) != 0)
            {
                string requestFilePath = this.GetRequestedServiceID(url);

                if (!string.IsNullOrEmpty(requestFilePath))
                {
                    Basics.ServicePathInfo rServicePathInfo =
                        Basics.ServicePathInfo.Parse(requestFilePath, false);

                    if (string.IsNullOrEmpty(rServicePathInfo.ServiceID))
                        return null;

                    return rServicePathInfo;
                }

                return Basics.ServicePathInfo.Parse(workingInstance.Settings.Configurations.DefaultPage, false);
            }

            return null;
        }

        private Basics.IDomain SearchChildrenThatOverrides(ref Basics.IDomain workingInstance, ref Basics.Context.IURL url)
        {
            if (workingInstance == null)
                return null;

            List<string> childDomainIDAccessTree = new List<string>();
            childDomainIDAccessTree.AddRange(workingInstance.IDAccessTree);

            foreach (Basics.DomainInfo childDI in workingInstance.Children)
            {
                childDomainIDAccessTree.Add(childDI.ID);

                Basics.IDomain rDomainInstance =
                    new Domain(childDomainIDAccessTree.ToArray(), this.Domain.Language.ID);

                Basics.ServicePathInfo servicePathInfo =
                    this.TryResolveURL(ref rDomainInstance, url);
                if (servicePathInfo == null)
                {
                    childDomainIDAccessTree.RemoveAt(childDomainIDAccessTree.Count - 1);

                    continue;
                }

                if (rDomainInstance.Settings.Services.ServiceItems.GetServiceItem(servicePathInfo.FullPath) == null)
                {
                    if (rDomainInstance.Children.Count > 0)
                    {
                        rDomainInstance = this.SearchChildrenThatOverrides(ref rDomainInstance, ref url);

                        if (rDomainInstance != null)
                            return rDomainInstance;
                    }

                    if (servicePathInfo.IsMapped)
                    {
                        url = this._Context.Request.Header.URL;

                        return workingInstance;
                    }
                }
                else
                    return rDomainInstance;

                rDomainInstance.Dispose();
                childDomainIDAccessTree.RemoveAt(childDomainIDAccessTree.Count - 1);
            }

            return null;
        }

        private void RectifyRequestPath(Basics.URLMapping.ResolvedMapped resolvedMapped)
        {
            string requestURL = string.Format(
                "{0}{1}",
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                resolvedMapped.ServicePathInfo.FullPath
            );
            if (resolvedMapped.URLQueryDictionary.Count > 0)
                requestURL = string.Concat(requestURL, "?", resolvedMapped.URLQueryDictionary.ToString());

            // Let the server understand what this URL is about...
            this._Context.Request.RewritePath(requestURL);
        }

        public void OverrideDomain(string[] domainIDAccessTree, string languageID) =>
            this.Domain = new Domain(domainIDAccessTree, languageID);

        public void ProvideXeoraJSStream(ref Stream outputStream)
        {
            outputStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    string.Format("Xeora.Web._sps_v{0}.js", this.XeoraJSVersion));
        }

        public Basics.Execution.BindInfo GetxSocketBind()
        {
            Basics.Execution.BindInfo rBindInfo = null;

            if (this.ServiceType == Basics.ServiceTypes.xSocket &&
                !string.IsNullOrEmpty(this._ExecuteIn))
                rBindInfo = Basics.Execution.BindInfo.Make(this._ExecuteIn);

            return rBindInfo;
        }

        public void RenderService(Basics.ControlResult.Message messageResult, string updateBlockControlID)
        {
            if (this.ServicePathInfo == null)
            {
                string systemMessage = this.Domain.Language.Get("TEMPLATE_IDMUSTBESET");

                if (string.IsNullOrEmpty(systemMessage))
                    systemMessage = Global.SystemMessages.TEMPLATE_IDMUSTBESET;

                throw new System.Exception(systemMessage + "!");
            }

            switch (this.ServiceType)
            {
                case Basics.ServiceTypes.Template:
                    this.ServiceResult = this.Domain.Render(this.ServicePathInfo, messageResult, updateBlockControlID);

                    break;
                case Basics.ServiceTypes.xService:
                    if (this.IsAuthenticationRequired)
                    {
                        Basics.xService.Parameters PostedExecuteParameters =
                            new Basics.xService.Parameters(this._Context.Request.Body.Form["xParams"]);

                        if (PostedExecuteParameters.PublicKey != null)
                        {
                            this.IsAuthenticationRequired = false;

                            foreach (string AuthKey in this._AuthenticationKeys)
                            {
                                if (this.Domain.xService.ReadSessionVariable(PostedExecuteParameters.PublicKey, AuthKey) == null)
                                {
                                    this.IsAuthenticationRequired = true;

                                    break;
                                }
                            }
                        }
                    }

                    if (!this.IsAuthenticationRequired)
                        this.ServiceResult = this.Domain.xService.RenderxService(this._ExecuteIn, this.ServicePathInfo.ServiceID);
                    else
                    {
                        object MethodResult = new SecurityException(Global.SystemMessages.XSERVICE_AUTH);

                        this.ServiceResult = this.Domain.xService.GeneratexServiceXML(MethodResult);
                    }

                    break;
            }
        }

        public void ProvideFileStream(string requestedFilePath, out Stream outputStream)
        {
            Domain workingInstance = (Domain)this.Domain;
            do
            {
                workingInstance.ProvideFileStream(requestedFilePath, out outputStream);

                if (outputStream == null)
                    workingInstance = (Domain)workingInstance.Parent;
            } while (workingInstance != null && outputStream == null);
        }

        public void PushLanguageChange(string languageID)
        {
            // Make the language Persist
            Basics.Context.IHttpCookieInfo languageCookie =
                this._Context.Request.Header.Cookie[this._CookieSearchKeyForLanguage];

            if (languageCookie == null)
                languageCookie = this._Context.Response.Header.Cookie.CreateNewCookie(this._CookieSearchKeyForLanguage);

            languageCookie.Value = languageID;
            languageCookie.Expires = DateTime.Now.AddDays(30);

            this._Context.Response.Header.Cookie.AddOrUpdate(languageCookie);
            //!---

            ((Domain)this.Domain).PushLanguageChange(languageID);
        }

        public Basics.URLMapping.ResolvedMapped QueryURLResolver(string requestFilePath)
        {
            Basics.IDomain dummy = null;
            return this.QueryURLResolver(ref dummy, requestFilePath);
        }

        private Basics.URLMapping.ResolvedMapped QueryURLResolver(ref Basics.IDomain workingInstance, string requestFilePath)
        {
            if (workingInstance == null)
                workingInstance = this.Domain;

            if (workingInstance != null &&
                workingInstance.Settings.URLMappings.IsActive &&
                !string.IsNullOrEmpty(workingInstance.Settings.URLMappings.ResolverExecutable))
            {
                Basics.Execution.BindInfo resolverBindInfo =
                    Basics.Execution.BindInfo.Make(string.Format("{0}?URLResolver,rfp", workingInstance.Settings.URLMappings.ResolverExecutable));
                resolverBindInfo.PrepareProcedureParameters(
                     new Basics.Execution.BindInfo.ProcedureParser(
                         (ref Basics.Execution.BindInfo.ProcedureParameter procedureParameter) =>
                         {
                             procedureParameter.Value = requestFilePath;
                         }
                     )
                 );
                resolverBindInfo.InstanceExecution = true;

                Basics.Execution.BindInvokeResult<Basics.URLMapping.ResolvedMapped> ResolverBindInvokeResult =
                    Manager.AssemblyCore.InvokeBind<Basics.URLMapping.ResolvedMapped>(resolverBindInfo, Manager.ExecuterTypes.Undefined);

                if (ResolverBindInvokeResult.Exception == null)
                    return ResolverBindInvokeResult.Result;
            }

            return null;
        }

        // Cache for performance consideration
        private static Basics.DomainInfo.DomainInfoCollection _AvailableDomains = null;
        public Basics.DomainInfo.DomainInfoCollection GetAvailableDomains()
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

            Basics.DomainInfo.DomainInfoCollection rDomainInfoCollection =
                new Basics.DomainInfo.DomainInfoCollection();

            foreach (DirectoryInfo dI in domainDI.GetDirectories())
            {
                try
                {
                    Basics.DomainInfo.LanguageInfo[] languages =
                        Deployment.DomainDeployment.AvailableLanguageInfos(new string[] { dI.Name });

                    Deployment.DomainDeployment domainDeployment =
                        Deployment.InstanceFactory.Current.GetOrCreate(new string[] { dI.Name }, languages[0].ID);

                    Basics.DomainInfo domainInfo =
                        new Basics.DomainInfo(domainDeployment.DeploymentType, dI.Name, languages);
                    domainInfo.Children.AddRange(domainDeployment.Children);

                    rDomainInfoCollection.Add(domainInfo);
                }
                catch (System.Exception ex)
                {
                    Helper.EventLogger.Log(ex);
                }
            }
            DomainControl._AvailableDomains = rDomainInfoCollection;

            return DomainControl._AvailableDomains;
        }

        public void Dispose()
        {
            this.Domain.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
