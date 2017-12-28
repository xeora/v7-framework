using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Xeora.Web.Basics.Context;

namespace Xeora.Web.Basics
{
    public class Helpers
    {
        public static string CreateURL(string serviceFullPath, params KeyValuePair<string, string>[] queryStrings) =>
            Helpers.CreateURL(true, serviceFullPath, queryStrings);

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

        public static IDomain CreateNewDomainInstance(string[] domainIDAccessTree) =>
            Helpers.CreateNewDomainInstance(domainIDAccessTree, null);

        public static IDomain CreateNewDomainInstance(string[] domainIDAccessTree, string domainLanguageID) =>
            (IDomain)Activator.CreateInstance(TypeCache.Instance.Domain, new object[] { domainIDAccessTree, domainLanguageID });

        public static string CurrentHandlerID =>
            (string)AppDomain.CurrentDomain.GetData(string.Format("HandlerID_{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));

        public static void AssignHandlerID(string handlerID) =>
            AppDomain.CurrentDomain.SetData(string.Format("HandlerID_{0}", System.Threading.Thread.CurrentThread.ManagedThreadId), handlerID);

        internal static IHandler HandlerInstance =>
            (IHandler)TypeCache.Instance.RemoteInvoke.InvokeMember("GetHandler", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { Helpers.CurrentHandlerID });

        public static IHttpContext Context => Helpers.HandlerInstance.Context;

        public static string SiteTitle
        {
            get => Helpers.HandlerInstance.DomainControl.SiteTitle;
            set => Helpers.HandlerInstance.DomainControl.SiteTitle = value;
        }

        public static string SiteIconURL
        {
            get => Helpers.HandlerInstance.DomainControl.SiteIconURL;
            set => Helpers.HandlerInstance.DomainControl.SiteIconURL = value;
        }

        public static IDomain CurrentDomainInstance => Helpers.HandlerInstance.DomainControl.Domain;

        public static void ProvideDomainContentsFileStream(string fileName, out Stream outputStream) =>
            Helpers.HandlerInstance.DomainControl.ProvideFileStream(fileName, out outputStream);

        public static void PushLanguageChange(string languageID) =>
            Helpers.HandlerInstance.DomainControl.PushLanguageChange(languageID);

        public static DomainInfo.DomainInfoCollection Domains =>
            Helpers.HandlerInstance.DomainControl.GetAvailableDomains();

        private static Service.IScheduledTaskEngine _ScheduledTasks = null;
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

        public static Service.VariablePoolOperation VariablePool =>
            new Service.VariablePoolOperation(Helpers.Context.Session.SessionID, Helpers.Context.HashCode);

        public static Service.VariablePoolOperation VariablePoolForxService =>
            new Service.VariablePoolOperation("000000000000000000000000", "00000001");
    }
}
