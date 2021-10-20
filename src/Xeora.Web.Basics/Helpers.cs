using System;
using System.Reflection;
using System.Collections.Generic;
using Xeora.Web.Basics.Context;
using Xeora.Web.Basics.Execution;
using System.Threading;
using Xeora.Web.Basics.Context.Request;

namespace Xeora.Web.Basics
{
    public class Helpers
    {
        internal static DomainPacket Packet { get; set; }
        internal static INegotiator Negotiator => 
            Helpers.Packet.Negotiator;

        /// <summary>
        /// Gets the context name of current execution
        /// </summary>
        /// <value>The value of context name of current execution</value>
        public static string Name => 
            Helpers.Packet.Name;
        
        /// <summary>
        /// Creates the Xeora Url with variable pool accessibility
        /// </summary>
        /// <returns>Xeora Url</returns>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStrings">Query string definitions (if any)</param>
        public static string CreateUrl(string serviceFullPath, params KeyValuePair<string, string>[] queryStrings) =>
            Helpers.CreateUrl(true, serviceFullPath, queryStrings);

        /// <summary>
        /// Creates the Xeora Url with variable pool accessibility
        /// </summary>
        /// <returns>Xeora Url</returns>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStringDictionary">Query string dictionary</param>
        public static string CreateUrl(string serviceFullPath, QueryStringDictionary queryStringDictionary = null) =>
            Helpers.CreateUrl(true, serviceFullPath, queryStringDictionary);

        /// <summary>
        /// Creates the Xeora Url with variable pool accessibility
        /// </summary>
        /// <returns>Xeora Url</returns>
        /// <param name="useSameVariablePool">If set to <c>true</c> uses same variable pool with the current request</param>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStrings">Query string definitions (if any)</param>
        public static string CreateUrl(bool useSameVariablePool, string serviceFullPath, params KeyValuePair<string, string>[] queryStrings) =>
            Helpers.CreateUrl(useSameVariablePool, serviceFullPath, QueryStringDictionary.Make(queryStrings));

        /// <summary>
        /// Creates the Xeora Url with variable pool accessibility
        /// </summary>
        /// <returns>Xeora Url</returns>
        /// <param name="useSameVariablePool">If set to <c>true</c> uses same variable pool with the current request</param>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStringDictionary">Query string dictionary</param>
        public static string CreateUrl(bool useSameVariablePool, string serviceFullPath, QueryStringDictionary queryStringDictionary = null)
        {
            string applicationRoot =
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;

            string rString = !useSameVariablePool 
                ? $"{applicationRoot}{serviceFullPath}" 
                : $"{applicationRoot}{Helpers.Context.HashCode}/{serviceFullPath}";

            if (queryStringDictionary != null && queryStringDictionary.Count > 0)
                rString = string.Concat(rString, "?", queryStringDictionary.ToString());

            return rString;
        }
        
        /// <summary>
        /// Send a Refresh request to the Xeora Engine to clean up
        /// cache and rebuild the available domains 
        /// </summary>
        public static void Refresh() =>
            Helpers.Negotiator.ClearCache();

        /// <summary>
        /// Resolves the service path info from Url
        /// </summary>
        /// <returns>The service path info</returns>
        /// <param name="requestFilePath">Request file path</param>
        public static ServiceDefinition ResolveServiceDefinitionFromUrl(string requestFilePath)
        {
            if (string.IsNullOrEmpty(requestFilePath))
                return null;

            Mapping.Url urlInstance = Mapping.Url.Current;

            if (urlInstance != null &&
                urlInstance.Active)
            {
                Mapping.MappingItem[] mappingItems =
                    urlInstance.Items.ToArray();

                foreach (Mapping.MappingItem mapItem in mappingItems)
                {
                    System.Text.RegularExpressions.Match rqMatch = 
                        System.Text.RegularExpressions.Regex.Match(
                            requestFilePath,
                            mapItem.RequestMap,
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );

                    if (rqMatch.Success)
                        return mapItem.ResolveEntry.ServiceDefinition;
                }
            }

            // Take Care Application Path and HashCode if it is exists work with application browser path
            // this comes /APPPATH(/path?somekey=withquery)?
            // or this /APPPATH/432432/(path?somekey=withquery)?
            // or this /Standart_tr-TR/somefile.png
            // take care of it!
            string currentDomainContentPath =
                Helpers.CurrentDomainInstance.ContentsVirtualPath;

            // first test if it is domain content path
            if (requestFilePath.IndexOf(currentDomainContentPath, StringComparison.Ordinal) == 0)
            {
                // This is a DomainContents Request
                // So no Template and also no default template usage
                return null;
            }

            string applicationRootPath =
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            System.Text.RegularExpressions.Match mR =
                System.Text.RegularExpressions.Regex.Match(
                    requestFilePath,
                    $"{applicationRootPath}(\\d+/)?"
                );
            if (mR.Success && mR.Index == 0)
                requestFilePath = requestFilePath.Remove(0, mR.Length);

            // Check if there is any query string exists! if so, template will be till there. 
            if (requestFilePath.IndexOf('?') > -1)
                requestFilePath = requestFilePath.Substring(0, requestFilePath.IndexOf('?'));

            return string.IsNullOrEmpty(requestFilePath)
                ? ServiceDefinition.Parse(Helpers.CurrentDomainInstance.Settings.Configurations.DefaultTemplate, false)
                : ServiceDefinition.Parse(requestFilePath, false);
        }

        /// <summary>
        /// Creates the new domain instance
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIdAccessTree">DomainId Access tree</param>
        public static Domain.IDomain CreateNewDomainInstance(string[] domainIdAccessTree) =>
            Helpers.CreateNewDomainInstance(domainIdAccessTree, null);

        /// <summary>
        /// Creates the new domain instance with a specific language
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIdAccessTree">DomainId Access tree</param>
        /// <param name="domainLanguageId">Domain language identifier</param>
        public static Domain.IDomain CreateNewDomainInstance(string[] domainIdAccessTree, string domainLanguageId) =>
            Helpers.Negotiator.CreateNewDomainInstance(domainIdAccessTree, domainLanguageId);

        /// <summary>
        /// Gets the current thread handler identifier
        /// </summary>
        /// <value>The current handler identifier</value>
        public static string CurrentHandlerId =>
            (string)AppDomain.CurrentDomain.GetData($"HandlerId_{Thread.CurrentThread.ManagedThreadId}");

        /// <summary>
        /// Assigns the handler identifier for the current thread
        /// </summary>
        /// <param name="handlerId">Handler identifier</param>
        public static void AssignHandlerId(string handlerId) =>
            AppDomain.CurrentDomain.SetData($"HandlerId_{Thread.CurrentThread.ManagedThreadId}", handlerId);

        /// <summary>
        /// Keeps the current handler and prevent removal at the end of the request
        /// </summary>
        public static void KeepCurrentHandler() =>
            Helpers.Negotiator.KeepHandler(Helpers.CurrentHandlerId);
            
        /// <summary>
        /// Drops the current handler and immediately remove from the memory without waiting the compilation for the request
        /// </summary>
        public static void DropCurrentHandler() =>
            Helpers.Negotiator.DropHandler(Helpers.CurrentHandlerId);
        
        internal static IHandler HandlerInstance =>
            Helpers.Negotiator.GetHandler(Helpers.CurrentHandlerId);

        /// <summary>
        /// Gets the Http Context
        /// </summary>
        /// <value>Http Context</value>
        public static IHttpContext Context => Helpers.HandlerInstance.Context;

        /// <summary>
        /// Gets or sets the site title html tag value
        /// </summary>
        /// <value>The value of site title html tag</value>
        public static string SiteTitle
        {
            get => Helpers.HandlerInstance.DomainControl.SiteTitle;
            set => Helpers.HandlerInstance.DomainControl.SiteTitle = value;
        }

        /// <summary>
        /// Gets or sets the site favicon Url value
        /// </summary>
        /// <value>The value of site favicon Url</value>
        public static string SiteIconUrl
        {
            get => Helpers.HandlerInstance.DomainControl.SiteIconUrl;
            set => Helpers.HandlerInstance.DomainControl.SiteIconUrl = value;
        }

        /// <summary>
        /// Gets the current domain instance
        /// </summary>
        /// <value>The current domain instance</value>
        public static Domain.IDomain CurrentDomainInstance => Helpers.HandlerInstance.DomainControl.Domain;

        /// <summary>
        /// Gets the available domains of Xeora Projects
        /// </summary>
        /// <value>Xeora project domains</value>
        public static Domain.Info.DomainCollection Domains =>
            Helpers.HandlerInstance.DomainControl.GetAvailableDomains();

        /// <summary>
        /// Gets Task Scheduler Engine
        /// </summary>
        /// <value>Task Scheduler Engine instance</value>
        public static Service.ITaskSchedulerEngine TaskScheduler =>
            Helpers.Negotiator.TaskScheduler;

        /// <summary>
        /// Gets Status Tracker Instance
        /// </summary>
        /// <value>Status Tracker instance</value>
        public static IStatusTracker StatusTracker =>
            Helpers.Negotiator.StatusTracker;

        /// <summary>
        /// Gets the variable pool
        /// </summary>
        /// <value>Variable pool instance</value>
        public static Service.VariablePoolOperation VariablePool =>
            new Service.VariablePoolOperation(Helpers.Context.Session.SessionId, Helpers.Context.HashCode);

        /// <summary>
        /// Gets the variable pool for xService
        /// </summary>
        /// <value>Variable pool instance for xService.</value>
        public static Service.VariablePoolOperation VariablePoolForxService =>
            new Service.VariablePoolOperation("000000000000000000000000", "00000001");

        /// <summary>
        /// Calls the side Xeora executable belongs to the current domain, to the side domain, or to the sub domains
        /// </summary>
        /// <returns>Call result</returns>
        /// <param name="executable">Xeora executable name</param>
        /// <param name="classes">Class name</param>
        /// <param name="procedure">Procedure name</param>
        /// <param name="parameterValues">Parameters values</param>
        /// <typeparam name="T">Expected result Type</typeparam>
        public static InvokeResult<T> CrossCall<T>(string executable, string[] classes, string procedure, params object[] parameterValues)
        {
            Assembly webManagerAsm = Assembly.Load("Xeora.Web.Manager");
            Type assemblyCoreType =
                webManagerAsm.GetType("Xeora.Web.Manager.AssemblyCore", false, true);

            Bind bind = null;
            Dictionary<string, object> parametersValueMap =
                new Dictionary<string, object>();

            if (parameterValues == null ||
                parameterValues.Length == 0)
                bind = Bind.Make($"{executable}?{string.Join(".", classes)}.{procedure}");
            else
            {
                string[] parametersStructure = new string[parameterValues.Length];

                for (int pC = 0; pC < parameterValues.Length; pC++)
                {
                    string paramName = $"PARAM{pC}";

                    parametersValueMap[paramName] = parameterValues[pC];
                    parametersStructure[pC] = paramName;
                }
                bind =
                    Bind.Make(
                        string.Format(
                            "{0}?{1}.{2},{3}",
                            executable,
                            string.Join(".", classes),
                            procedure,
                            string.Join("|", parametersStructure)
                        )
                    );
            }

            bind.Parameters.Prepare(
                param => parametersValueMap[param.Key] 
            );

            try
            {
                MethodInfo invokeBindMethod =
                    assemblyCoreType.GetMethod("InvokeBind", new [] { typeof(HttpMethod), typeof(Bind) });
                if (invokeBindMethod == null) throw new ArgumentNullException();
                
                invokeBindMethod = invokeBindMethod.MakeGenericMethod(typeof(T));

                return
                    (InvokeResult<T>)invokeBindMethod.Invoke(null, new object[] { Helpers.Context.Request.Header.Method, bind });
            }
            catch (Exception ex)
            {
                InvokeResult<T> rInvokeResult =
                    new InvokeResult<T>(bind)
                    {
                        Exception = new Exception("CrossCall Execution Error!", ex)
                    };

                return rInvokeResult;
            }
        }
    }
}
