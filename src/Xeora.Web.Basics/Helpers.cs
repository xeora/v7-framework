using System;
using System.Reflection;
using System.Collections.Generic;
using Xeora.Web.Basics.Context;
using Xeora.Web.Basics.Execution;
using System.Threading;

namespace Xeora.Web.Basics
{
    public class Helpers
    {
        /// <summary>
        /// Creates the Xeora URL with variable pool accessibilities
        /// </summary>
        /// <returns>Xeora URL</returns>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStrings">Query string definitions (if any)</param>
        public static string CreateURL(string serviceFullPath, params KeyValuePair<string, string>[] queryStrings) =>
            Helpers.CreateURL(true, serviceFullPath, queryStrings);

        /// <summary>
        /// Creates the Xeora URL with variable pool accessibilities
        /// </summary>
        /// <returns>Xeora URL</returns>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStringDictionary">Query string dictionary</param>
        public static string CreateURL(string serviceFullPath, QueryStringDictionary queryStringDictionary = null) =>
            Helpers.CreateURL(true, serviceFullPath, queryStringDictionary);

        /// <summary>
        /// Creates the Xeora URL with variable pool accessibilities
        /// </summary>
        /// <returns>Xeora URL</returns>
        /// <param name="useSameVariablePool">If set to <c>true</c> uses same variable pool with the current request</param>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStrings">Query string definitions (if any)</param>
        public static string CreateURL(bool useSameVariablePool, string serviceFullPath, params KeyValuePair<string, string>[] queryStrings) =>
            Helpers.CreateURL(useSameVariablePool, serviceFullPath, QueryStringDictionary.Make(queryStrings));

        /// <summary>
        /// Creates the Xeora URL with variable pool accessibilities
        /// </summary>
        /// <returns>Xeora URL</returns>
        /// <param name="useSameVariablePool">If set to <c>true</c> uses same variable pool with the current request</param>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStringDictionary">Query string dictionary</param>
        public static string CreateURL(bool useSameVariablePool, string serviceFullPath, QueryStringDictionary queryStringDictionary = null)
        {
            string rString = null;

            string applicationRoot =
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;

            if (!useSameVariablePool)
                rString = string.Format("{0}{1}", applicationRoot, serviceFullPath);
            else
                rString = string.Format("{0}{1}/{2}", applicationRoot, Helpers.Context.HashCode, serviceFullPath);

            if (queryStringDictionary != null && queryStringDictionary.Count > 0)
                rString = string.Concat(rString, "?", queryStringDictionary.ToString());

            return rString;
        }

        /// <summary>
        /// Resolves the service path info from URL
        /// </summary>
        /// <returns>The service path info</returns>
        /// <param name="requestFilePath">Request file path</param>
        public static ServiceDefinition ResolveServiceDefinitionFromURL(string requestFilePath)
        {
            if (string.IsNullOrEmpty(requestFilePath))
                return null;

            Mapping.URL urlInstance = Mapping.URL.Current;

            if (urlInstance != null &&
                urlInstance.Active)
            {
                Mapping.MappingItem[] mappingItems =
                    urlInstance.Items.ToArray();
                System.Text.RegularExpressions.Match rqMatch = null;

                foreach (Mapping.MappingItem mapItem in mappingItems)
                {
                    rqMatch =
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
            if (requestFilePath.IndexOf(currentDomainContentPath) == 0)
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
                    string.Format("{0}(\\d+/)?", applicationRootPath)
                );
            if (mR.Success && mR.Index == 0)
                requestFilePath = requestFilePath.Remove(0, mR.Length);

            // Check if there is any query string exists! if so, template will be till there. 
            if (requestFilePath.IndexOf('?') > -1)
                requestFilePath = requestFilePath.Substring(0, requestFilePath.IndexOf('?'));

            if (string.IsNullOrEmpty(requestFilePath))
                return ServiceDefinition.Parse(Helpers.CurrentDomainInstance.Settings.Configurations.DefaultTemplate, false);

            return ServiceDefinition.Parse(requestFilePath, false);
        }

        /// <summary>
        /// Creates the new domain instance
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIDAccessTree">DomainID Access tree</param>
        public static Domain.IDomain CreateNewDomainInstance(string[] domainIDAccessTree) =>
            Helpers.CreateNewDomainInstance(domainIDAccessTree, null);

        /// <summary>
        /// Creates the new domain instance with a specific language
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIDAccessTree">DomainID Access tree</param>
        /// <param name="domainLanguageID">Domain language identifier</param>
        public static Domain.IDomain CreateNewDomainInstance(string[] domainIDAccessTree, string domainLanguageID) =>
            (Domain.IDomain)Activator.CreateInstance(TypeCache.Current.Domain, new object[] { domainIDAccessTree, domainLanguageID });

        /// <summary>
        /// Gets the current thread handler identifier
        /// </summary>
        /// <value>The current handler identifier</value>
        public static string CurrentHandlerID =>
            (string)AppDomain.CurrentDomain.GetData(string.Format("HandlerID_{0}", Thread.CurrentThread.ManagedThreadId));

        /// <summary>
        /// Assigns the handler identifier for the current thread
        /// </summary>
        /// <param name="handlerID">Handler identifier</param>
        public static void AssignHandlerID(string handlerID) =>
            AppDomain.CurrentDomain.SetData(string.Format("HandlerID_{0}", Thread.CurrentThread.ManagedThreadId), handlerID);

        internal static IHandler HandlerInstance =>
            (IHandler)TypeCache.Current.RemoteInvoke.InvokeMember("GetHandler", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { Helpers.CurrentHandlerID });

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
        /// Gets or sets the site favicon URL value
        /// </summary>
        /// <value>The value of site favicon URL</value>
        public static string SiteIconURL
        {
            get => Helpers.HandlerInstance.DomainControl.SiteIconURL;
            set => Helpers.HandlerInstance.DomainControl.SiteIconURL = value;
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
            
        private static object _ScheduledTasksLock = new object();
        private static Service.IScheduledTaskEngine _ScheduledTasks = null;
        /// <summary>
        /// Gets Task Scheduler Engine
        /// </summary>
        /// <value>Task Scheduler Engine instance</value>
        public static Service.IScheduledTaskEngine ScheduledTasks
        {
            get
            {
                Monitor.Enter(Helpers._ScheduledTasksLock);
                try
                {
                    if (Helpers._ScheduledTasks == null)
                        Helpers._ScheduledTasks =
                           (Service.IScheduledTaskEngine)TypeCache.Current.RemoteInvoke.InvokeMember("GetScheduledTaskEngine", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
                }
                finally
                {
                    Monitor.Exit(Helpers._ScheduledTasksLock);
                }

                return Helpers._ScheduledTasks;
            }
        }

        private static object _StatusTrackerLock = new object();
        private static IStatusTracker _StatusTracker = null;
        /// <summary>
        /// Gets Status Tracker Instance
        /// </summary>
        /// <value>Status Tracker instance</value>
        public static IStatusTracker StatusTracker
        {
            get
            {
                Monitor.Enter(Helpers._StatusTrackerLock);
                try
                {
                    if (Helpers._StatusTracker == null)
                        Helpers._StatusTracker =
                           (IStatusTracker)TypeCache.Current.StatusTracker.InvokeMember("Current", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);
                }
                finally
                {
                    Monitor.Exit(Helpers._StatusTrackerLock);
                }

                return Helpers._StatusTracker;
            }
        }

        /// <summary>
        /// Gets the variable pool
        /// </summary>
        /// <value>Variable pool instance</value>
        public static Service.VariablePoolOperation VariablePool =>
            new Service.VariablePoolOperation(Helpers.Context.Session.SessionID, Helpers.Context.HashCode);

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
            Assembly webManagerAsm = Assembly.Load("Xeora.Web");
            Type assemblyCoreType =
                webManagerAsm.GetType("Xeora.Web.Manager.AssemblyCore", false, true);

            Bind bind = null;
            Dictionary<string, object> parametersValueMap =
                new Dictionary<string, object>();

            if (parameterValues == null ||
                parameterValues.Length == 0)
                bind = Bind.Make(string.Format("{0}?{1}.{2}", executable, string.Join(".", classes), procedure));
            else
            {
                string[] parametersStructure = new string[parameterValues.Length];

                for (int pC = 0; pC < parameterValues.Length; pC++)
                {
                    string paramName = string.Format("PARAM{0}", pC);

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
                (ProcedureParameter param) => parametersValueMap[param.Key] 
            );

            try
            {
                MethodInfo invokeBindMethod =
                    assemblyCoreType.GetMethod("InvokeBind", new Type[] { typeof(HttpMethod), typeof(Bind) });
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
