using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Context;
using Xeora.Web.Basics.ControlResult;
using Xeora.Web.Basics.X;
using Xeora.Web.Controller.Directive;
using Xeora.Web.Site;

namespace Xeora.Web.Handler
{
    public class XeoraHandler : IHandler
    {
        private readonly bool _ForceRefresh;

        private DateTime _BeginRequestTime;
        private bool _SupportCompression;

        private DomainControl _DomainControl = null;

        internal XeoraHandler(ref IHttpContext context, bool forceRefresh)
        {
            this._ForceRefresh = forceRefresh;

            this.Context = context ?? throw new System.Exception("Context is required!");
            this.HandlerID = Guid.NewGuid().ToString();

            // Check URL contains ApplicationRootPath (~) or SiteRootPath (¨) modifiers
            string RootPath =
                System.Web.HttpUtility.UrlDecode(this.Context.Request.Header.URL.Raw);

            if (RootPath.IndexOf("~/") > -1)
            {
                int tildeIdx = RootPath.IndexOf("~/");

                RootPath = RootPath.Remove(0, tildeIdx + 2);
                RootPath = RootPath.Insert(0, Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);

                this.Context.Request.RewritePath(RootPath);
            }
            else if (RootPath.IndexOf("¨/") > -1)
            {
                // It search something outside of XeoraCube Handler
                int helfIdx = RootPath.IndexOf("¨/");

                RootPath = RootPath.Remove(0, helfIdx + 2);
                RootPath = RootPath.Insert(0, Basics.Configurations.Xeora.Application.Main.VirtualRoot);

                this.Context.Request.RewritePath(RootPath);
            }
            // !--

            Helpers.AssignHandlerID(this.HandlerID);
        }

        public string HandlerID { get; private set; }
        public IHttpContext Context { get; private set; }
        public IDomainControl DomainControl => this._DomainControl;

        public void Handle()
        {
            this._BeginRequestTime = DateTime.Now;
            this._SupportCompression = false;

            try
            {
                IHttpContext context = this.Context;
                this._DomainControl = new DomainControl(ref context);
                if (this._ForceRefresh)
                    this._DomainControl.Domain.ClearCache();

                Basics.Enum.PageCachingTypes defaultCaching =
                    this._DomainControl.Domain.Settings.Configurations.DefaultCaching;

                // Caching Settings
                if (defaultCaching != Basics.Enum.PageCachingTypes.AllContent &&
                    defaultCaching != Basics.Enum.PageCachingTypes.AllContentCookiless)
                {
                    switch (defaultCaching)
                    {
                        case Basics.Enum.PageCachingTypes.NoCache:
                        case Basics.Enum.PageCachingTypes.NoCacheCookiless:
                            this.Context.Response.Header.AddOrUpdate("Cache-Control", "no-store, must-revalidate");

                            break;
                        default:
                            this.Context.Response.Header.AddOrUpdate("Cache-Control", "no-cache");
                            this.Context.Response.Header.AddOrUpdate("Pragma", "no-cache");

                            break;
                    }

                    this.Context.Response.Header.AddOrUpdate("Expires", "0");
                }
                else
                    this.Context.Response.Header.AddOrUpdate("Expires", DateTime.Now.AddMonths(1).ToString("r"));
                // !---

                string acceptEncodings = this.Context.Request.Header["Accept-Encoding"];
                if (Configurations.Xeora.Application.Main.Compression && acceptEncodings != null)
                    this._SupportCompression = (acceptEncodings.IndexOf("gzip") > -1);

                if (this._DomainControl.ServiceDefinition == null)
                    this.HandleStaticFile(); // Static File that has the same level of Application folder or Domain Content File
                else
                    this.HandleServiceRequest(); // Service Request (Template, xService, xSocket)
            }
            catch (System.Exception ex)
            {
                this.Context.Response.Header.Status.Code = 500;

                this.HandleErrorLogging(ex);
            }
            finally
            {
                // If Redirection has been assigned, handle it
                if (this.Context["RedirectLocation"] != null)
                {
                    if (((string)this.Context["RedirectLocation"]).IndexOf("://") == -1)
                    {
                        string redirectLocation =
                            string.Format("{0}://{1}{2}", Configurations.Xeora.Service.Ssl ? "https" : "http", this.Context.Request.Header.Host, this.Context["RedirectLocation"]);

                        this.Context.AddOrUpdate("RedirectLocation", redirectLocation);
                    }

                    if (this.Context.Request.Header["X-BlockRenderingID"] == null)
                        this.Context.Response.Redirect((string)this.Context["RedirectLocation"]);
                    else
                    {
                        this.Context.Response.Header.Status.Code = 200;

                        byte[] redirectBytes =
                            Encoding.UTF8.GetBytes(string.Format("rl:{0}", (string)this.Context["RedirectLocation"]));

                        this.Context.Response.Header.AddOrUpdate("Content-Type", "text/html");
                        this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

                        this.Context.Response.Write(redirectBytes, 0, redirectBytes.Length);
                    }
                }
            }
        }

        private void HandleStaticFile()
        {
            string domainContentsPath =
                this._DomainControl.Domain.ContentsVirtualPath;
            string requestedFileVirtualPath =
                this.Context.Request.Header.URL.RelativePath;
            
            int dcpIndex = requestedFileVirtualPath.IndexOf(domainContentsPath);
            if (dcpIndex == -1)
            {
                // This is also not a request for default DomainContents

                // Extract the ChildDomainIDAccessTree and LanguageID using RequestPath
                string requestedDomainWebPath = requestedFileVirtualPath;
                string browserImplementation =
                    Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
                int impIndex = requestedDomainWebPath.IndexOf(browserImplementation);
                if (impIndex > -1)
                    requestedDomainWebPath = requestedDomainWebPath.Remove(impIndex, browserImplementation.Length);

                if (requestedDomainWebPath.IndexOf(this.Context.HashCode) == 0)
                    requestedDomainWebPath = requestedDomainWebPath.Substring(this.Context.HashCode.Length + 1);

                if (requestedDomainWebPath.IndexOf('/') > -1)
                {
                    requestedDomainWebPath = requestedDomainWebPath.Split('/')[0];

                    if (!string.IsNullOrEmpty(requestedDomainWebPath))
                    {
                        string[] splittedRequestedDomainWebPath = requestedDomainWebPath.Split('_');

                        if (splittedRequestedDomainWebPath.Length == 2)
                        {
                            string[] childDomainIDAccessTree = null;
                            string childDomainLanguageID = string.Empty;

                            childDomainIDAccessTree = splittedRequestedDomainWebPath[0].Split('-');
                            childDomainLanguageID = splittedRequestedDomainWebPath[1];

                            this._DomainControl.OverrideDomain(childDomainIDAccessTree, childDomainLanguageID);

                            domainContentsPath = this._DomainControl.Domain.ContentsVirtualPath;
                            dcpIndex = requestedFileVirtualPath.IndexOf(domainContentsPath);
                        }
                    }
                }
            }

            // Provide Requested File Stream
            if (dcpIndex > -1)
            {
                // This is a well known Domain Content file
                // Clean Domain Contents pointer
                requestedFileVirtualPath =
                    requestedFileVirtualPath.Remove(0, dcpIndex + domainContentsPath.Length + 1);

                this.PostDomainContentFileToClient(requestedFileVirtualPath);

                return;
            }

            string scriptFileName =
                string.Format("_bi_sps_v{0}.js", this._DomainControl.XeoraJSVersion);
            int scriptFileNameIndex =
                requestedFileVirtualPath.IndexOf(scriptFileName);
            bool isScriptRequesting =
                scriptFileNameIndex > -1 && (requestedFileVirtualPath.Length - scriptFileName.Length) == scriptFileNameIndex;

            if (isScriptRequesting)
            {
                this.PostBuildInJavaScriptToClient();

                return;
            }

            this.PostRequestedStaticFileToClient();
        }

        private void HandleServiceRequest()
        {
            if (this._DomainControl.IsAuthenticationRequired)
                this.RedirectToAuthenticationPage(this._DomainControl.ServiceDefinition.FullPath);
            else
            {
                switch (this._DomainControl.ServiceType)
                {
                    case Basics.Domain.ServiceTypes.Template:
                        this.HandleTemplateRequest();

                        break;
                    case Basics.Domain.ServiceTypes.xService:
                        this.CreateServiceResult(null);

                        break;
                    case Basics.Domain.ServiceTypes.xSocket:
                        this.HandlexSocketRequest();

                        break;
                }

            }
        }

        private void HandleTemplateRequest()
        {
            Message messageResult = null;
            string methodResult = string.Empty;
            string bindInformation =
                this.Context.Request.Body.Form[string.Format("_sys_bind_{0}", this.Context.HashCode)];

            if (this.Context.Request.Header.Method == HttpMethod.POST &&
                !string.IsNullOrEmpty(bindInformation))
            {
                // Decode Encoded Call Function to Readable
                Basics.Execution.Bind bind =
                    Basics.Execution.Bind.Make(
                        Manager.AssemblyCore.DecodeFunction(bindInformation));

                bind.Parameters.Prepare(
                    (parameter) =>
                    {
                        Property property = new Property(0, parameter.Query, null);
                        property.InstanceRequested += (ref Basics.Domain.IDomain instance) => instance = this._DomainControl.Domain;
                        property.Setup();

                        property.Render(null);

                        return property.ObjectResult;
                    }
                );

                Basics.Execution.InvokeResult<object> invokeResult =
                    Manager.AssemblyCore.InvokeBind<object>(Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Undefined);

                if (invokeResult.Exception != null)
                    messageResult = new Message(invokeResult.Exception.ToString());
                else if (invokeResult.Result != null && invokeResult.Result is Message)
                    messageResult = (Message)invokeResult.Result;
                else if (invokeResult.Result != null && invokeResult.Result is RedirectOrder)
                    this.Context.AddOrUpdate("RedirectLocation", ((RedirectOrder)invokeResult.Result).Location);
                else
                    methodResult = Manager.AssemblyCore.GetPrimitiveValue(invokeResult.Result);
            }

            if (string.IsNullOrEmpty((string)this.Context["RedirectLocation"]))
            {
                // Create HashCode for request and apply to URL
                if (this.Context.Request.Header.Method == HttpMethod.GET)
                {
                    string applicationRootPath =
                        Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
                    string currentURL = this.Context.Request.Header.URL.RelativePath;
                    currentURL = currentURL.Remove(0, currentURL.IndexOf(applicationRootPath));

                    System.Text.RegularExpressions.Match mR =
                        System.Text.RegularExpressions.Regex.Match(currentURL, string.Format("{0}\\d+/", applicationRootPath));

                    // Not assigned, so assign!
                    if (!mR.Success)
                    {
                        string tailURL = this.Context.Request.Header.URL.RelativePath;
                        tailURL = tailURL.Remove(0, tailURL.IndexOf(applicationRootPath) + applicationRootPath.Length);

                        string rewrittenPath =
                            string.Format("{0}{1}/{2}", applicationRootPath, this.Context.HashCode, tailURL);

                        if (!string.IsNullOrEmpty(this.Context.Request.Header.URL.QueryString))
                            rewrittenPath = string.Format("{0}?{1}", rewrittenPath, this.Context.Request.Header.URL.QueryString);

                        this.Context.Request.RewritePath(rewrittenPath);
                    }
                }

                this.CreateTemplateResult(messageResult, methodResult);
            }
        }

        private void HandlexSocketRequest()
        {
            this.Context.Response.Header.AddOrUpdate("Content-Type", this._DomainControl.ServiceMimeType);
            this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

            // Decode Encoded Call Function to Readable
            Basics.Execution.Bind bind =
                this._DomainControl.GetxSocketBind();

            bind.Parameters.Prepare(
                (parameter) =>
                {
                    Property property = new Property(0, parameter.Query, null);
                    property.InstanceRequested += (ref Basics.Domain.IDomain instance) => instance = this._DomainControl.Domain;
                    property.Setup();

                    property.Render(null);

                    return property.ObjectResult;
                }
            );

            List<KeyValuePair<string, object>> keyValueList = new List<KeyValuePair<string, object>>();
            foreach (Basics.Execution.ProcedureParameter item in bind.Parameters)
                keyValueList.Add(new KeyValuePair<string, object>(item.Key, item.Value));

            IHttpContext context = this.Context;
            SocketObject xSocketObject =
                new SocketObject(ref context, keyValueList.ToArray());

            bind.Parameters.Override(new string[] { "xso" });
            bind.Parameters.Prepare(
                (parameter) => xSocketObject
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.AssemblyCore.InvokeBind<object>(Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Undefined);

            if (invokeResult.Exception != null)
                throw new Exception.ServiceSocketException(invokeResult.Exception.ToString());

            if (invokeResult.Result is Message messageResult)
            {
                if (messageResult.Type == Message.Types.Error)
                    throw new Exception.ServiceSocketException(messageResult.Content);
            }
        }

        private void HandleErrorLogging(System.Exception exception)
        {
            // Prepare For Exception List
            StringBuilder exceptionClientView =
                new StringBuilder();

            exceptionClientView.AppendLine("-- APPLICATION EXCEPTION --");
            exceptionClientView.Append(exception.ToString());
            exceptionClientView.AppendLine();
            // ----

            StringBuilder exceptionLogging =
                new StringBuilder();

            // -- Session Log Text
            exceptionLogging.AppendLine("-- Session Variables --");
            string[] sessionKeys =
                this.Context.Session.Keys;

            if (sessionKeys != null)
            {
                try
                {
                    foreach (string key in sessionKeys)
                        exceptionLogging.AppendLine(string.Format(" {0} -> {1}", key, this.Context.Session[key]));
                }
                catch (System.Exception ex)
                {
                    // The collection was modified after the enumerator was created.

                    exceptionLogging.AppendLine(string.Format(" Exception Occured -> {0}", ex.Message));
                }
            }
            // !--

            exceptionLogging.AppendLine();

            // -- Request Log Text
            exceptionLogging.AppendLine("-- Request POST Variables --");
            foreach (string key in this.Context.Request.Body.Form.Keys)
                exceptionLogging.AppendLine(string.Format(" {0} -> {1}", key, this.Context.Request.Body.Form[key]));
            exceptionLogging.AppendLine();
            exceptionLogging.AppendLine("-- Request URL & Query String --");
            exceptionLogging.AppendLine(string.Format("{0}?{1}", this.Context.Request.Header.URL.RelativePath, this.Context.Request.Header.URL.QueryString));
            exceptionLogging.AppendLine();
            exceptionLogging.AppendLine("-- Error Content --");
            exceptionLogging.Append(exception.ToString());

            Helper.EventLogger.Log(exceptionLogging.ToString());

            StringBuilder outputSB =
                new StringBuilder();

            if (Configurations.Xeora.Application.Main.Debugging)
            {
                // It is debugging, that's why it is safe to push everything to client
                outputSB.AppendFormat("<h2 align=\"center\" style=\"color:#CC0000\">{0}!</h2>", Global.SystemMessages.SYSTEM_ERROROCCURED);
                outputSB.Append("<hr size=\"1px\">");
                outputSB.AppendFormat("<pre>{0}</pre>", exceptionClientView.ToString());

                byte[] outputBytes = Encoding.UTF8.GetBytes(outputSB.ToString());

                this.Context.Response.Header.AddOrUpdate("Content-Type", "text/html");
                this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

                this.Context.Response.Write(outputBytes, 0, outputBytes.Length);

                return;
            }

            if (this._DomainControl != null)
            {
                this.Context.AddOrUpdate("RedirectLocation",
                    string.Format("{0}://{1}{2}",
                        Configurations.Xeora.Service.Ssl ? "https" : "http",
                        this.Context.Request.Header.Host,
                        Helpers.CreateURL(false, this._DomainControl.Domain.Settings.Configurations.DefaultPage)
                    )
                );

                return;
            }

            // If unrecoverable, push the error message to the user
            outputSB.AppendFormat("<h2 align=\"center\" style=\"color:#CC0000\">{0}!</h2>", Global.SystemMessages.SYSTEM_ERROROCCURED);
            outputSB.AppendFormat("<h4 align=\"center\">{0}</h4>", exception.Message);

            byte[] OutputBytes = Encoding.UTF8.GetBytes(outputSB.ToString());

            this.Context.Response.Header.AddOrUpdate("Content-Type", "text/html");
            this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

            this.Context.Response.Write(OutputBytes, 0, OutputBytes.Length);
        }

        private bool IsRequestedStaticFileBanned(string requestFilePath)
        {
            requestFilePath = requestFilePath.Replace(Configurations.Xeora.Application.Main.PhysicalRoot, string.Empty);

            foreach (string bannedRegEx in Configurations.Xeora.Application.BannedFiles)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(requestFilePath, bannedRegEx, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        private void PostRequestedStaticFileToClient()
        {
            string requestFilePath =
                string.Concat(
                    Configurations.Xeora.Application.Main.PhysicalRoot,
                    this.Context.Request.Header.URL.RelativePath
                );
            requestFilePath = Path.GetFullPath(requestFilePath);

            if (!File.Exists(requestFilePath) ||
                (File.Exists(requestFilePath) && this.IsRequestedStaticFileBanned(requestFilePath)))
            {
                this.Context.Response.Header.Status.Code = 404;
                this.Context.AddOrUpdate("RedirectLocation", null);

                return;
            }

            string contentType =
                MimeType.GetMime(Path.GetExtension(requestFilePath));

            string range = this.Context.Request.Header["Range"];
            bool isPartialRequest = !string.IsNullOrEmpty(range);

            Stream requestFileStream = null;

            if (!isPartialRequest)
            {
                this.Context.Response.Header.AddOrUpdate("Accept-Ranges", "bytes");

                try
                {
                    requestFileStream = new FileStream(requestFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    this.Context.Response.Header.AddOrUpdate("Content-Length", requestFileStream.Length.ToString());

                    this.WriteOutput(contentType, ref requestFileStream, false);
                }
                catch (System.Exception)
                {
                    throw;
                }
                finally
                {
                    if (requestFileStream != null)
                    {
                        requestFileStream.Close();
                        GC.SuppressFinalize(requestFileStream);
                    }
                }

                this.Context.AddOrUpdate("RedirectLocation", null);

                return;
            }

            long beginRange = 0, endRange = -1;

            if (range.IndexOf("bytes=") == 0)
            {
                range = range.Remove(0, "bytes=".Length);
                try
                {
                    if (!long.TryParse(range.Split('-')[0], out beginRange))
                        beginRange = 0;
                    if (!long.TryParse(range.Split('-')[1], out endRange))
                        endRange = -1;
                }
                catch (System.Exception)
                {
                    this.Context.Response.Header.Status.Code = 416;
                    this.Context.AddOrUpdate("RedirectLocation", null);

                    return;
                }
            }

            try
            {
                requestFileStream =
                    new FileStream(requestFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (endRange == -1)
                    endRange = requestFileStream.Length - 1;

                long requestingLength = endRange - beginRange + 1;

                if (endRange > requestFileStream.Length - 1 || beginRange > endRange)
                {
                    this.Context.Response.Header.Status.Code = 416;
                    this.Context.AddOrUpdate("RedirectLocation", null);

                    return;
                }

                this.Context.Response.Header.Status.Code = 206;
                this.Context.Response.Header.AddOrUpdate("Content-Type", contentType);
                this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");
                this.Context.Response.Header.AddOrUpdate("Content-Range", string.Format("bytes {0}-{1}/{2}", beginRange, endRange, requestFileStream.Length));
                this.Context.Response.Header.AddOrUpdate("Content-Length", requestingLength.ToString());

                requestFileStream.Seek(beginRange, SeekOrigin.Begin);

                byte[] buffer = new byte[102400];
                int bR = 0;
                do
                {
                    bR = requestFileStream.Read(buffer, 0, buffer.Length);

                    if (requestingLength < bR)
                        bR = (int)requestingLength;

                    this.Context.Response.Write(buffer, 0, bR);

                    requestingLength -= bR;
                } while (requestingLength != 0 && bR != 0);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (requestFileStream != null)
                {
                    requestFileStream.Close();
                    GC.SuppressFinalize(requestFileStream);
                }
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void PostDomainContentFileToClient(string requestedFilePathInDomainContents)
        {
            Stream requestFileStream = null;
            try
            {
                this._DomainControl.Domain.ProvideFileStream(requestedFilePathInDomainContents, out requestFileStream);

                this.WriteOutput(
                    MimeType.GetMime(
                        Path.GetExtension(requestedFilePathInDomainContents)
                    ),
                    ref requestFileStream,
                    this._SupportCompression
                );
            }
            catch (FileNotFoundException)
            {
                this.Context.Response.Header.Status.Code = 404;
                return;
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (requestFileStream != null)
                {
                    requestFileStream.Close();
                    GC.SuppressFinalize(requestFileStream);
                }
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void PostBuildInJavaScriptToClient()
        {
            Stream requestFileStream = null;
            this._DomainControl.ProvideXeoraJSStream(ref requestFileStream);

            try
            {
                this.WriteOutput(
                    MimeType.GetMime(".js"),
                    ref requestFileStream,
                    this._SupportCompression
                );
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (requestFileStream != null)
                {
                    requestFileStream.Close();
                    GC.SuppressFinalize(requestFileStream);
                }
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void RedirectToAuthenticationPage(string currentRequestedTemplate = null)
        {
            switch (this._DomainControl.ServiceType)
            {
                case Basics.Domain.ServiceTypes.Template:
                    // Get AuthenticationPage 
                    KeyValuePair<string, string> referrerURLQueryString;
                    string authenticationPage =
                        this._DomainControl.Domain.Settings.Configurations.AuthenticationPage;

                    if (!string.IsNullOrEmpty(currentRequestedTemplate) &&
                        string.Compare(authenticationPage, currentRequestedTemplate, true) != 0)
                        referrerURLQueryString =
                            new KeyValuePair<string, string>(
                                "xcRef",
                                System.Web.HttpUtility.UrlEncode(this.Context.Request.Header.URL.Raw)
                            );

                    // Reset Redirect Location to AuthenticationPage
                    this.Context.AddOrUpdate("RedirectLocation",
                        Helpers.CreateURL(true, authenticationPage, referrerURLQueryString));

                    break;
                case Basics.Domain.ServiceTypes.xService:
                    this.CreateServiceResult(null);

                    break;
            }
        }

        private void CreateServiceResult(Message messageResult)
        {
            this._DomainControl.RenderService(messageResult, null);

            StringWriter writer = null;
            try
            {
                writer = new StringWriter();
                writer.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                writer.Write(this._DomainControl.ServiceResult.Content);
                writer.Flush();
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            this.WriteOutput(this._DomainControl.ServiceMimeType, writer.ToString(), this._SupportCompression);
        }

        private void CreateTemplateResult(Message messageResult, string methodResult)
        {
            string updateBlockControlID =
                this.Context.Request.Header["X-BlockRenderingID"];

            if (!string.IsNullOrEmpty(updateBlockControlID))
                this._DomainControl.RenderService(messageResult, updateBlockControlID.Split('>'));
            else
                this._DomainControl.RenderService(messageResult, null);

            if (this.Context.Response.Header.Status.Code == 200 && this._DomainControl.ServiceResult.HasErrors)
                this.Context.Response.Header.Status.Code = 218;

            StringBuilder sB = new StringBuilder();

            sB.Append(this._DomainControl.ServiceResult.Content);
            sB.Append(methodResult);

            string result = sB.ToString();
            string sys_RenderDurationMark = "<!--_sys_PAGERENDERDURATION-->";
            int idxRenderDurationMark =
                result.IndexOf(sys_RenderDurationMark);
            if (idxRenderDurationMark > -1)
            {
                TimeSpan EndRequestTimeSpan = DateTime.Now.Subtract(this._BeginRequestTime);

                result = result.Remove(idxRenderDurationMark, sys_RenderDurationMark.Length);
                result = result.Insert(idxRenderDurationMark, EndRequestTimeSpan.TotalMilliseconds.ToString());
            }

            if (!this._DomainControl.IsWorkingAsStandAlone &&
                string.IsNullOrEmpty(updateBlockControlID))
            {
                StringWriter writer = null;
                try
                {
                    writer = new StringWriter();
                    this.CreateHTMLTag(ref writer, result);
                    writer.Flush();

                    result = writer.ToString();
                }
                catch (System.Exception)
                {
                    throw;
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }

            this.WriteOutput(this._DomainControl.ServiceMimeType, result, this._SupportCompression);
        }

        private void CreateHTMLTag(ref StringWriter writer, string bodyContent)
        {
            if (Configurations.Xeora.Application.Main.UseHTML5Header)
                writer.WriteLine("<!doctype html>");
            else
                writer.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");

            writer.WriteLine("<html>");

            this.AppendHeadTag(ref writer);
            this.AppendBodyTag(ref writer, bodyContent);

            writer.WriteLine("</html>");
        }

        private void AppendHeadTag(ref StringWriter writer)
        {
            writer.WriteLine("<head>");

            this.AppendMetaTags(ref writer);

            writer.WriteLine(
                string.Format(
                    "<title>{0}</title>",
                    this._DomainControl.SiteTitle
                )
            );

            if (!string.IsNullOrEmpty(this._DomainControl.SiteIconURL))
            {
                writer.WriteLine(
                    string.Format(
                        "<link href=\"{0}\" rel=\"shortcut icon\">",
                        this._DomainControl.SiteIconURL
                    )
                );
            }

            writer.WriteLine(
                string.Format(
                    "<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}/styles.css\" />",
                    this._DomainControl.Domain.ContentsVirtualPath
                )
            );

            writer.WriteLine(
                string.Format(
                    "<script type=\"text/javascript\" src=\"{0}_bi_sps_v{1}.js\"></script>",
                    Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                    this._DomainControl.XeoraJSVersion
                )
            );
            writer.WriteLine(
                string.Format(
                    "<script type=\"text/javascript\">__XeoraJS.pushCode({0});</script>",
                    this.Context.HashCode
                )
            );

            writer.WriteLine("</head>");
        }

        private void AppendMetaTags(ref StringWriter writer)
        {
            bool isContentTypeAdded = false, isPragmaAdded = false, isCacheControlAdded = false, isExpiresAdded = false;

            foreach (KeyValuePair<Basics.MetaRecord.Tags, string> kVP in this._DomainControl.MetaRecord.CommonTags)
            {
                switch (Basics.MetaRecord.QueryTagSpace(kVP.Key))
                {
                    case Basics.MetaRecord.TagSpaces.name:
                        writer.WriteLine(
                            string.Format(
                                "<meta name=\"{0}\" content=\"{1}\" />",
                                Basics.MetaRecord.GetTagHtmlName(kVP.Key),
                                kVP.Value
                            )
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.httpequiv:
                        writer.WriteLine(
                            string.Format(
                                "<meta http-equiv=\"{0}\" content=\"{1}\" />",
                                Basics.MetaRecord.GetTagHtmlName(kVP.Key),
                                kVP.Value
                            )
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.property:
                        writer.WriteLine(
                            string.Format(
                                "<meta property=\"{0}\" content=\"{1}\" />",
                                Basics.MetaRecord.GetTagHtmlName(kVP.Key),
                                kVP.Value
                            )
                        );

                        break;
                }

                switch (kVP.Key)
                {
                    case Basics.MetaRecord.Tags.contenttype:
                        isContentTypeAdded = true;

                        break;
                    case Basics.MetaRecord.Tags.pragma:
                        isPragmaAdded = true;

                        break;
                    case Basics.MetaRecord.Tags.cachecontrol:
                        isCacheControlAdded = true;

                        break;
                    case Basics.MetaRecord.Tags.expires:
                        isExpiresAdded = true;

                        break;
                }
            }

            string keyName = string.Empty;
            foreach (KeyValuePair<string, string> kVP in this._DomainControl.MetaRecord.CustomTags)
            {
                keyName = kVP.Key;
                switch (Basics.MetaRecord.QueryTagSpace(ref keyName))
                {
                    case Basics.MetaRecord.TagSpaces.name:
                        writer.WriteLine(
                            string.Format(
                                "<meta name=\"{0}\" content=\"{1}\" />",
                                keyName,
                                kVP.Value
                            )
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.httpequiv:
                        writer.WriteLine(
                            string.Format(
                                "<meta http-equiv=\"{0}\" content=\"{1}\" />",
                                keyName,
                                kVP.Value
                            )
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.property:
                        writer.WriteLine(
                            string.Format(
                                "<meta property=\"{0}\" content=\"{1}\" />",
                                keyName,
                                kVP.Value
                            )
                        );

                        break;
                }
            }

            if (!isContentTypeAdded)
            {
                writer.WriteLine(
                    string.Format(
                        "<meta http-equiv=\"Content-Type\" content=\"{0}; charset={1}\" />",
                        this._DomainControl.ServiceMimeType,
                        Encoding.UTF8.WebName
                    )
                );
            }

            Basics.Enum.PageCachingTypes defaultType =
                this._DomainControl.Domain.Settings.Configurations.DefaultCaching;

            if (defaultType == Basics.Enum.PageCachingTypes.NoCache || defaultType == Basics.Enum.PageCachingTypes.NoCacheCookiless)
            {
                if (!isPragmaAdded)
                    writer.WriteLine("<meta http-equiv=\"Pragma\" content=\"no-cache\" />");
                if (!isCacheControlAdded)
                    writer.WriteLine("<meta http-equiv=\"Cache-Control\" content=\"no-cache\" />");
                if (!isExpiresAdded)
                    writer.WriteLine("<meta http-equiv=\"Expires\" content=\"0\" />");
            }
        }

        private void AppendBodyTag(ref StringWriter writer, string bodyContent)
        {
            writer.WriteLine("<body>");
            writer.WriteLine(
                string.Format(
                    "<form method=\"post\" action=\"{0}{1}/{2}?{3}\" enctype=\"multipart/form-data\" style=\"margin: 0px; padding: 0px;\">",
                    Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                    this.Context.HashCode,
                    this._DomainControl.ServiceDefinition.FullPath,
                    this.Context.Request.Header.URL.QueryString
                )
            );
            writer.WriteLine(
                string.Format(
                    "<input type=\"hidden\" name=\"_sys_bind_{0}\" id=\"_sys_bind_{0}\" />",
                    this.Context.HashCode
                )
            );

            writer.Write(bodyContent);

            writer.WriteLine("</form>");
            writer.WriteLine("</body>");
        }

        private void CompressUnsafe(ref Stream outputStream, out Stream gzippedStream)
        {
            byte[] contentBuffer = new byte[102400];
            int bC = 0;

            GZipStream gzipCompression = null;
            try
            {
                gzippedStream = new MemoryStream();
                gzipCompression = new GZipStream(gzippedStream, CompressionMode.Compress, true);

                do
                {
                    bC = outputStream.Read(contentBuffer, 0, contentBuffer.Length);

                    gzipCompression.Write(contentBuffer, 0, bC);
                } while (bC > 0);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (gzipCompression != null)
                {
                    gzipCompression.Close();
                    GC.SuppressFinalize(gzipCompression);
                }
            }
        }

        private void WriteOutput(string contentType, string outputContent, bool sendAsCompressed)
        {
            if (this.Context["RedirectLocation"] != null)
                return;

            Stream outputStream = null;
            try
            {
                outputStream =
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(outputContent));

                this.WriteOutput(contentType, ref outputStream, sendAsCompressed);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Close();
                    GC.SuppressFinalize(outputStream);
                }
            }
        }

        private void WriteOutput(string contentType, ref Stream outputStream, bool sendAsCompressed)
        {
            this.Context.Response.Header.AddOrUpdate("Content-Type", contentType);

            if (sendAsCompressed)
            {
                Stream gzippedStream = null;
                try
                {
                    this.CompressUnsafe(ref outputStream, out gzippedStream);

                    if (gzippedStream != null && gzippedStream.Length < outputStream.Length)
                    {
                        this.Context.Response.Header.AddOrUpdate("Content-Encoding", "gzip");
                        this.WriteToSocketUnsafe(ref gzippedStream);

                        return;
                    }
                }
                catch (System.Exception)
                {
                    throw;
                }
                finally
                {
                    if (gzippedStream != null)
                    {
                        gzippedStream.Close();
                        GC.SuppressFinalize(gzippedStream);
                    }
                }
            }

            this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");
            this.WriteToSocketUnsafe(ref outputStream);
        }

        private void WriteToSocketUnsafe(ref Stream outputStream)
        {
            long bandwidth =
                Configurations.Xeora.Application.Main.Bandwidth;
            bool applyBandwidthRules = bandwidth > 0;
            if (bandwidth <= 0)
                bandwidth = 102400;

            byte[] contentBuffer = new byte[bandwidth];
            int bC = 0;

            outputStream.Seek(0, SeekOrigin.Begin);
            do
            {
                bC = outputStream.Read(contentBuffer, 0, contentBuffer.Length);

                if (bC > 0)
                {
                    this.Context.Response.Write(contentBuffer, 0, bC);

                    if (applyBandwidthRules && bC == bandwidth)
                        Thread.Sleep(1000);
                }
            } while (bC != 0);
        }
    }
}
