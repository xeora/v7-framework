using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Xeora.Web.Basics.Context;

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
        /// <param name="useSameVariablePool">If set to <c>true</c> uses same variable pool with the current request</param>
        /// <param name="serviceFullPath">Valid Xeora Service Full Path</param>
        /// <param name="queryStrings">Query string definitions (if any)</param>
        public static string CreateURL(bool useSameVariablePool, string serviceFullPath, params KeyValuePair<string, string>[] queryStrings)
        {
            string rString = null;

            URLQueryDictionary urlQueryDictionary = URLQueryDictionary.Make(queryStrings);
            string applicationRoot =
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;

            if (!useSameVariablePool)
                rString = string.Format("{0}{1}", applicationRoot, serviceFullPath);
            else
                rString = string.Format("{0}{1}/{2}", applicationRoot, Helpers.Context.HashCode, serviceFullPath);

            if (urlQueryDictionary.Count > 0)
                rString = string.Concat(rString, "?", urlQueryDictionary.ToString());

            return rString;
        }

        /// <summary>
        /// Resolves the service path info from URL
        /// </summary>
        /// <returns>The service path info</returns>
        /// <param name="requestFilePath">Request file path</param>
        public static ServicePathInfo ResolveServicePathInfoFromURL(string requestFilePath)
        {
            if (string.IsNullOrEmpty(requestFilePath))
                return null;

            URLMapping urlMappingInstance = URLMapping.Current;

            if (urlMappingInstance != null &&
                urlMappingInstance.IsActive)
            {
                URLMapping.URLMappingItem[] urlMappingItems =
                    urlMappingInstance.Items.ToArray();
                System.Text.RegularExpressions.Match rqMatch = null;

                foreach (URLMapping.URLMappingItem urlMapItem in urlMappingItems)
                {
                    rqMatch =
                        System.Text.RegularExpressions.Regex.Match(
                            requestFilePath,
                            urlMapItem.RequestMap,
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );

                    if (rqMatch.Success)
                        return urlMapItem.ResolveInfo.ServicePathInfo;
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
                return ServicePathInfo.Parse(Helpers.CurrentDomainInstance.Settings.Configurations.DefaultPage, false);

            return ServicePathInfo.Parse(requestFilePath, false);
        }

        /// <summary>
        /// Creates the new domain instance
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIDAccessTree">DomainID Access tree</param>
        public static IDomain CreateNewDomainInstance(string[] domainIDAccessTree) =>
            Helpers.CreateNewDomainInstance(domainIDAccessTree, null);

        /// <summary>
        /// Creates the new domain instance with a specific language
        /// </summary>
        /// <returns>The new domain instance</returns>
        /// <param name="domainIDAccessTree">DomainID Access tree</param>
        /// <param name="domainLanguageID">Domain language identifier</param>
        public static IDomain CreateNewDomainInstance(string[] domainIDAccessTree, string domainLanguageID) =>
            (IDomain)Activator.CreateInstance(TypeCache.Instance.Domain, new object[] { domainIDAccessTree, domainLanguageID });

        /// <summary>
        /// Gets the current thread handler identifier
        /// </summary>
        /// <value>The current handler identifier</value>
        public static string CurrentHandlerID =>
            (string)AppDomain.CurrentDomain.GetData(string.Format("HandlerID_{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));

        /// <summary>
        /// Assigns the handler identifier for the current thread
        /// </summary>
        /// <param name="handlerID">Handler identifier</param>
        public static void AssignHandlerID(string handlerID) =>
            AppDomain.CurrentDomain.SetData(string.Format("HandlerID_{0}", System.Threading.Thread.CurrentThread.ManagedThreadId), handlerID);

        internal static IHandler HandlerInstance =>
            (IHandler)TypeCache.Instance.RemoteInvoke.InvokeMember("GetHandler", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { Helpers.CurrentHandlerID });

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
        public static IDomain CurrentDomainInstance => Helpers.HandlerInstance.DomainControl.Domain;

        /// <summary>
        /// Provides the domain contents file stream
        /// </summary>
        /// <param name="fileName">File path and name to read</param>
        /// <param name="outputStream">Output stream</param>
        public static void ProvideDomainContentsFileStream(string fileName, out Stream outputStream) =>
            Helpers.HandlerInstance.DomainControl.ProvideFileStream(fileName, out outputStream);

        /// <summary>
        /// Pushs the language identifier to change the language of the current domain instance of the request
        /// </summary>
        /// <param name="languageID">Domain language identifier</param>
        public static void PushLanguageChange(string languageID) =>
            Helpers.HandlerInstance.DomainControl.PushLanguageChange(languageID);

        /// <summary>
        /// Gets the available domains of Xeora Projects
        /// </summary>
        /// <value>Xeora project domains</value>
        public static DomainInfo.DomainInfoCollection Domains =>
            Helpers.HandlerInstance.DomainControl.GetAvailableDomains();

        private static Service.IScheduledTaskEngine _ScheduledTasks = null;
        /// <summary>
        /// Gets Task Scheduler Engine
        /// </summary>
        /// <value>Task Scheduler Engine instance</value>
        public static Service.IScheduledTaskEngine ScheduledTasks
        {
            get
            {
                if (Helpers._ScheduledTasks == null)
                    Helpers._ScheduledTasks =
                       (Service.IScheduledTaskEngine)TypeCache.Instance.RemoteInvoke.InvokeMember("GetScheduledTaskEngine", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);

                return Helpers._ScheduledTasks;
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
    }
}
