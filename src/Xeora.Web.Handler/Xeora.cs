﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Xeora.Web.Basics;
using Xeora.Web.Basics.Context;
using Xeora.Web.Basics.ControlResult;
using Xeora.Web.Basics.X;
using Xeora.Web.Directives;
using Xeora.Web.Application;
using Xeora.Web.Basics.Context.Request;

namespace Xeora.Web.Handler
{
    public class Xeora : IHandler
    {
        private readonly bool _ForceRefresh;

        private DateTime _BeginRequestTime;
        private bool _SupportCompression;

        private DomainControl _DomainControl;

        internal Xeora(IHttpContext context, bool forceRefresh)
        {
            this._ForceRefresh = forceRefresh;

            this.Context = context ?? throw new Exception("Context is required!");
            this.HandlerId = Guid.NewGuid().ToString();
            Helpers.AssignHandlerId(this.HandlerId);
            
            // Check Url contains ApplicationRootPath (~) or SiteRootPath (¨) modifiers
            string rootPath =
                System.Web.HttpUtility.UrlDecode(context.Request.Header.Url.Raw);
            if (string.IsNullOrEmpty(rootPath)) return;

            if (this.CheckTilde(rootPath)) return;

            this.CheckHelf(rootPath);
            // !--
        }
        
        internal Xeora(IWebSocketContext context)
        {
            this.WebSocket = context ?? throw new Exception("Context is required!");
            this.HandlerId = Guid.NewGuid().ToString();
            Helpers.AssignHandlerId(this.HandlerId);
            
            // Check Url contains ApplicationRootPath (~) or SiteRootPath (¨) modifiers
            string rootPath =
                System.Web.HttpUtility.UrlDecode(context.Request.Header.Url.Raw);
            if (string.IsNullOrEmpty(rootPath)) return;

            if (this.CheckTilde(rootPath)) return;

            this.CheckHelf(rootPath);
            // !--
        }

        private bool CheckTilde(string rootPath)
        {
            if (rootPath.IndexOf("~/", StringComparison.Ordinal) < 0) return false;
            
            int tildeIdx = rootPath.IndexOf("~/", StringComparison.Ordinal);

            rootPath = rootPath.Remove(0, tildeIdx + 2);
            rootPath = rootPath.Insert(0, Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation);

            this.Context.Request.RewritePath(rootPath);

            return true;
        }
        
        private bool CheckHelf(string rootPath)
        {
            if (rootPath.IndexOf("¨/", StringComparison.Ordinal) < 0) return false;
            
            // It search something outside of Xeora Handler
            int helfIdx = rootPath.IndexOf("¨/", StringComparison.Ordinal);

            rootPath = rootPath.Remove(0, helfIdx + 2);
            rootPath = rootPath.Insert(0, Configurations.Xeora.Application.Main.VirtualRoot);

            this.Context.Request.RewritePath(rootPath);

            return true;
        }

        public string HandlerId { get; }
        private IWebSocketContext WebSocket { get; }
        public IHttpContext Context { get; }
        public IDomainControl DomainControl => this._DomainControl;

        public bool Handle()
        {
            this._BeginRequestTime = DateTime.Now;
            this._SupportCompression = false;

            try
            {
                if (this._ForceRefresh)
                    Application.DomainControl.ClearCache();

                this._DomainControl =
                    this.WebSocket == null
                        ? new DomainControl(this.Context)
                        : new DomainControl(this.WebSocket);

                if (this.WebSocket != null)
                {
                    this.HandleServiceRequest(); // Service Request (Template, xService, xSocket, webSocket)
                    return true;
                }

                Basics.Enum.PageCachingTypes defaultCaching =
                    this._DomainControl.Domain.Settings.Configurations.DefaultCaching;

                // Caching Settings
                if (defaultCaching != Basics.Enum.PageCachingTypes.AllContent &&
                    defaultCaching != Basics.Enum.PageCachingTypes.AllContentCookieless)
                {
                    switch (defaultCaching)
                    {
                        case Basics.Enum.PageCachingTypes.NoCache:
                        case Basics.Enum.PageCachingTypes.NoCacheCookieless:
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
                    this._SupportCompression = acceptEncodings.IndexOf("gzip", StringComparison.Ordinal) > -1;

                if (this._DomainControl.ServiceDefinition == null)
                    this.HandleStaticFile(); // Static File that has the same level of Application folder or Domain Content File
                else
                    this.HandleServiceRequest(); // Service Request (Template, xService, xSocket, webSocket)

                return true;
            }
            catch (Exception ex)
            {
                if (this.Context == null) return false;
                
                this.Context.Response.Header.Status.Code = 500;
                this.HandleErrorLogging(ex);

                return false;
            }
            finally
            {
                // If Redirection has been assigned, handle it
                if (this.Context?["RedirectLocation"] != null)
                {
                    if (((string)this.Context["RedirectLocation"]).IndexOf("://", StringComparison.InvariantCulture) == -1)
                    {
                        string redirectLocation =
                            $"{(Configurations.Xeora.Service.Ssl ? "https" : "http")}://{this.Context.Request.Header.Host}{this.Context["RedirectLocation"]}";

                        this.Context.AddOrUpdate("RedirectLocation", redirectLocation);
                    }

                    if (this.Context.Request.Header["X-BlockRenderingId"] == null)
                        this.Context.Response.Redirect((string)this.Context["RedirectLocation"]);
                    else
                    {
                        this.Context.Response.Header.Status.Code = 200;

                        byte[] redirectBytes =
                            Encoding.UTF8.GetBytes($"rl:{(string) this.Context["RedirectLocation"]}");

                        this.Context.Response.Header.AddOrUpdate("Content-Type", "text/html");
                        this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

                        this.Context.Response.Write(redirectBytes, 0, redirectBytes.Length);
                    }
                }
            }
        }

        private static readonly Regex DomainRequestPath =
            new Regex("^[a-zA-Z0-9]+(\\-[a-zA-Z0-9]+)*_[a-z]{2}\\-[A-Z]{2}$", RegexOptions.Compiled);
        private void HandleStaticFile()
        {
            string domainContentsPath =
                this._DomainControl.Domain.ContentsVirtualPath;
            string requestedFileVirtualPath =
                this.Context.Request.Header.Url.RelativePath;
            
            int dcpIndex = requestedFileVirtualPath.IndexOf(domainContentsPath, StringComparison.InvariantCulture);
            if (dcpIndex == -1)
            {
                // This is also not a request for default DomainContents

                // Extract the ChildDomainIdAccessTree and LanguageId using RequestPath
                // Searching something like: /Domain-Level1Child-Level2Child_en-US
                
                string requestedDomainWebPath = requestedFileVirtualPath;
                string browserImplementation =
                    Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
                int impIndex = requestedDomainWebPath.IndexOf(browserImplementation, StringComparison.InvariantCulture);
                if (impIndex > -1)
                    requestedDomainWebPath = requestedDomainWebPath.Remove(impIndex, browserImplementation.Length);

                if (requestedDomainWebPath.IndexOf(this.Context.HashCode, StringComparison.InvariantCulture) == 0)
                    requestedDomainWebPath = requestedDomainWebPath.Substring(this.Context.HashCode.Length + 1);

                int slashIndex = requestedDomainWebPath.IndexOf('/');
                if (slashIndex > -1)
                    requestedDomainWebPath = requestedDomainWebPath.Substring(0, slashIndex);

                if (Xeora.DomainRequestPath.Match(requestedDomainWebPath).Success)
                {
                    string[] requestedDomainWebPathParts = 
                        requestedDomainWebPath.Split('_');

                    string[] childDomainIdAccessTree = 
                        requestedDomainWebPathParts[0].Split('-');
                    string childDomainLanguageId = requestedDomainWebPathParts[1];

                    this._DomainControl.OverrideDomain(childDomainIdAccessTree, childDomainLanguageId);

                    domainContentsPath = this._DomainControl.Domain.ContentsVirtualPath;
                    dcpIndex = requestedFileVirtualPath.IndexOf(domainContentsPath, StringComparison.InvariantCulture);
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
                $"_bi_sps_v{Application.DomainControl.XeoraJsVersion}.js";
            int scriptFileNameIndex =
                requestedFileVirtualPath.IndexOf(scriptFileName, StringComparison.InvariantCulture);
            bool isScriptRequesting =
                scriptFileNameIndex > -1 && requestedFileVirtualPath.Length - scriptFileName.Length == scriptFileNameIndex;

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
            {
                this.RedirectToAuthenticationPage(this._DomainControl.ServiceDefinition.FullPath);
                return;
            }
            
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
                case Basics.Domain.ServiceTypes.WebSocket:
                    this.HandleWebSocketRequest();
                    
                    break;
            }
        }

        private void HandleTemplateRequest()
        {
            Message messageResult = null;
            string methodResult = string.Empty;
            string encryptedBindInformation =
                this.Context.Request.Body.Form[$"_sys_bind_{this.Context.HashCode}"];

            if (this.Context.Request.Header.Method == HttpMethod.POST &&
                !string.IsNullOrEmpty(encryptedBindInformation))
            {
                // Decode Encoded Call Function to Readable
                string bindInformation = 
                    this._DomainControl.Cryptography.Decrypt(encryptedBindInformation);
                if (string.IsNullOrEmpty(bindInformation))
                {
                    this.Context.AddOrUpdate(
                        "RedirectLocation", 
                        Helpers.CreateUrl(
                            false, 
                            this._DomainControl.Domain.Settings.Configurations.DefaultTemplate
                        )
                    );
                    return;
                }
                
                Basics.Execution.Bind bind =
                    Basics.Execution.Bind.Make(bindInformation);

                if (bind == null)
                    throw new Exception($"Bind information is not parsable: {bindInformation}");
                
                bind.Parameters.Prepare(
                    parameter => Property.Render(null, parameter.Query).Item2
                );

                Basics.Execution.InvokeResult<object> invokeResult =
                    Web.Manager.Executer.InvokeBind<object>(Helpers.Context.Request.Header.Method, bind, Web.Manager.ExecuterTypes.Undefined);

                if (invokeResult.Exception != null)
                    messageResult = new Message(invokeResult.Exception.ToString());
                else if (invokeResult.Result is Message message)
                    messageResult = message;
                else if (invokeResult.Result is RedirectOrder redirectOrder)
                    this.Context.AddOrUpdate("RedirectLocation", redirectOrder.Location);
                else
                    methodResult = Web.Manager.Executer.GetPrimitiveValue(invokeResult.Result);
            }

            if (!string.IsNullOrEmpty((string) this.Context["RedirectLocation"])) return;
            
            // Create HashCode for request and apply to Url
            if (this.Context.Request.Header.Method == HttpMethod.GET)
            {
                string applicationRootPath =
                    Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
                string currentUrl = this.Context.Request.Header.Url.RelativePath;
                currentUrl = currentUrl.Remove(0, currentUrl.IndexOf(applicationRootPath, StringComparison.InvariantCulture));

                Match mR =
                    Regex.Match(currentUrl, $"{applicationRootPath}\\d+/");

                // Not assigned, so assign!
                if (!mR.Success)
                {
                    string tailUrl = this.Context.Request.Header.Url.RelativePath;
                    tailUrl = tailUrl.Remove(0, tailUrl.IndexOf(applicationRootPath, StringComparison.InvariantCulture) + applicationRootPath.Length);

                    string rewrittenPath =
                        $"{applicationRootPath}{this.Context.HashCode}/{tailUrl}";

                    if (!string.IsNullOrEmpty(this.Context.Request.Header.Url.QueryString))
                        rewrittenPath = $"{rewrittenPath}?{this.Context.Request.Header.Url.QueryString}";

                    this.Context.Request.RewritePath(rewrittenPath);
                }
            }

            this.CreateTemplateResult(messageResult, methodResult);
        }

        private void HandlexSocketRequest()
        {
            this.Context.Response.Header.AddOrUpdate("Content-Type", this._DomainControl.ServiceMimeType);
            this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

            // Decode Encoded Call Function to Readable
            Basics.Execution.Bind bind =
                this._DomainControl.GetxSocketBind();

            bind.Parameters.Prepare(
                parameter => Property.Render(null, parameter.Query).Item2
            );

            List<KeyValuePair<string, object>> keyValueList = new List<KeyValuePair<string, object>>();
            foreach (Basics.Execution.ProcedureParameter item in bind.Parameters)
                keyValueList.Add(new KeyValuePair<string, object>(item.Key, item.Value));

            IHttpContext context = this.Context;
            SocketObject xSocketObject =
                new SocketObject(context, keyValueList.ToArray());

            bind.Parameters.Override(new [] { "xso" });
            bind.Parameters.Prepare(
                _ => xSocketObject
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Web.Manager.Executer.InvokeBind<object>(Helpers.Context.Request.Header.Method, bind, Web.Manager.ExecuterTypes.Undefined);

            if (invokeResult.Exception != null)
                throw new Exceptions.ServiceSocketException(invokeResult.Exception.ToString());

            if (!(invokeResult.Result is Message messageResult)) return;
            
            if (messageResult.Type == Message.Types.Error)
                throw new Exceptions.ServiceSocketException(messageResult.Content);
        }
        
        private void HandleWebSocketRequest()
        {
            // Decode Encoded Call Function to Readable
            Basics.Execution.Bind bind =
                this._DomainControl.GetxSocketBind();

            bind.Parameters.Prepare(
                parameter => Property.Render(null, parameter.Query).Item2
            );

            List<KeyValuePair<string, object>> keyValueList = new List<KeyValuePair<string, object>>();
            foreach (Basics.Execution.ProcedureParameter item in bind.Parameters)
                keyValueList.Add(new KeyValuePair<string, object>(item.Key, item.Value));
            
            bind.Parameters.Override(new [] { "wso", "parameters" });
            bind.Parameters.Prepare(
                parameter =>
                    parameter.Key switch
                    {
                        "wso" => this.WebSocket,
                        "parameters" => new WebSocketParameterCollection(keyValueList.ToArray()),
                        _ => null
                    }
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Web.Manager.Executer.InvokeBind<object>(HttpMethod.GET, bind, Web.Manager.ExecuterTypes.Undefined);

            if (invokeResult.Exception != null)
                throw new Exceptions.ServiceSocketException(invokeResult.Exception.ToString());

            if (invokeResult.Result is not Message messageResult) return;
            
            if (messageResult.Type == Message.Types.Error)
                throw new Exceptions.ServiceSocketException(messageResult.Content);
        }

        private void HandleErrorLogging(Exception exception)
        {
            // Prepare For Exception List
            StringBuilder exceptionClientView =
                new StringBuilder();

            exceptionClientView.AppendLine("-- APPLICATION EXCEPTION --");
            exceptionClientView.Append(exception);
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
                        exceptionLogging.AppendLine($" {key} -> {this.Context.Session[key]}");
                }
                catch (Exception ex)
                {
                    // The collection was modified after the enumerator was created.

                    exceptionLogging.AppendLine($" Exception Occured -> {ex.Message}");
                }
            }
            // !--

            exceptionLogging.AppendLine();

            // -- Request Log Text
            exceptionLogging.AppendLine("-- Request POST Variables --");
            foreach (string key in this.Context.Request.Body.Form.Keys)
                exceptionLogging.AppendLine($" {key} -> {this.Context.Request.Body.Form[key]}");
            exceptionLogging.AppendLine();
            exceptionLogging.AppendLine("-- Request Url & Query String --");
            exceptionLogging.AppendLine(
                $"{this.Context.Request.Header.Url.RelativePath}?{this.Context.Request.Header.Url.QueryString}");
            exceptionLogging.AppendLine();
            exceptionLogging.AppendLine("-- Error Content --");
            exceptionLogging.Append(exception);

            Basics.Console.Push("Execution Exception...", "Xeora Handler is FAILED!", exceptionLogging.ToString(), false, true, type: Basics.Console.Type.Error);

            StringBuilder outputStringBuilder =
                new StringBuilder();
            byte[] outputBytes;
            
            if (Configurations.Xeora.Application.Main.Debugging)
            {
                // It is debugging, that's why it is safe to push everything to client
                outputStringBuilder.AppendFormat("<h2 align=\"center\" style=\"color:#CC0000\">{0}!</h2>", Global.SystemMessages.SYSTEM_ERROROCCURED);
                outputStringBuilder.Append("<hr size=\"1px\">");
                outputStringBuilder.AppendFormat("<pre>{0}</pre>", exceptionClientView);

                outputBytes = Encoding.UTF8.GetBytes(outputStringBuilder.ToString());

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
                        Helpers.CreateUrl(false, this._DomainControl.Domain.Settings.Configurations.DefaultTemplate)
                    )
                );

                return;
            }

            // If unrecoverable, push the error message to the user
            outputStringBuilder.AppendFormat("<h2 align=\"center\" style=\"color:#CC0000\">{0}!</h2>", Global.SystemMessages.SYSTEM_ERROROCCURED);
            outputStringBuilder.AppendFormat("<h4 align=\"center\">{0}</h4>", exception.Message);

            outputBytes = Encoding.UTF8.GetBytes(outputStringBuilder.ToString());

            this.Context.Response.Header.AddOrUpdate("Content-Type", "text/html");
            this.Context.Response.Header.AddOrUpdate("Content-Encoding", "identity");

            this.Context.Response.Write(outputBytes, 0, outputBytes.Length);
        }

        private static bool IsRequestedStaticFileBanned(string requestFilePath)
        {
            requestFilePath = requestFilePath.Replace(Configurations.Xeora.Application.Main.PhysicalRoot, string.Empty);

            foreach (string bannedRegEx in Configurations.Xeora.Application.BannedFiles)
            {
                if (Regex.IsMatch(requestFilePath, bannedRegEx, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        private void PostRequestedStaticFileToClient()
        {
            string requestFilePath =
                string.Concat(
                    Configurations.Xeora.Application.Main.PhysicalRoot,
                    this.Context.Request.Header.Url.RelativePath
                );
            requestFilePath = Path.GetFullPath(requestFilePath);

            string contentType =
                MimeType.GetMime(Path.GetExtension(requestFilePath));
            
            if (!File.Exists(requestFilePath) ||
                File.Exists(requestFilePath) && Xeora.IsRequestedStaticFileBanned(requestFilePath))
            {
                this.Context.Response.Header.AddOrUpdate("Content-Type", contentType);
                this.Context.Response.Header.Status.Code = 404;
                this.Context.AddOrUpdate("RedirectLocation", null);

                return;
            }
            
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
                finally
                {
                    requestFileStream?.Dispose();
                }

                this.Context.AddOrUpdate("RedirectLocation", null);

                return;
            }

            long beginRange = 0, endRange = -1;

            if (range.IndexOf("bytes=", StringComparison.InvariantCulture) == 0)
            {
                range = range.Remove(0, "bytes=".Length);
                try
                {
                    if (!long.TryParse(range.Split('-')[0], out beginRange))
                        beginRange = 0;
                    if (!long.TryParse(range.Split('-')[1], out endRange))
                        endRange = -1;
                }
                catch (Exception)
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
                this.Context.Response.Header.AddOrUpdate("Content-Range",
                    $"bytes {beginRange}-{endRange}/{requestFileStream.Length}");
                this.Context.Response.Header.AddOrUpdate("Content-Length", requestingLength.ToString());

                requestFileStream.Seek(beginRange, SeekOrigin.Begin);

                byte[] buffer = new byte[102400];
                int bR;
                do
                {
                    bR = requestFileStream.Read(buffer, 0, buffer.Length);

                    if (requestingLength < bR)
                        bR = (int)requestingLength;

                    this.Context.Response.Write(buffer, 0, bR);

                    requestingLength -= bR;
                } while (requestingLength != 0 && bR != 0);
            }
            finally
            {
                requestFileStream?.Dispose();
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void PostDomainContentFileToClient(string requestedFilePathInDomainContents)
        {
            string contentType = 
                MimeType.GetMime(Path.GetExtension(requestedFilePathInDomainContents));
            
            Stream requestFileStream = null;
            try
            {
                this._DomainControl.Domain.ProvideFileStream(requestedFilePathInDomainContents, out requestFileStream);

                this.WriteOutput(contentType, ref requestFileStream, this._SupportCompression);
            }
            catch (FileNotFoundException)
            {
                this.Context.Response.Header.AddOrUpdate("Content-Type", contentType);
                this.Context.Response.Header.Status.Code = 404;
            }
            finally
            {
                requestFileStream?.Dispose();
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void PostBuildInJavaScriptToClient()
        {
            global::Xeora.Web.Application.DomainControl.ProvideXeoraJsStream(out Stream requestFileStream);

            try
            {
                this.WriteOutput(
                    MimeType.GetMime(".js"),
                    ref requestFileStream,
                    this._SupportCompression
                );
            }
            finally
            {
                requestFileStream?.Dispose();
            }

            this.Context.AddOrUpdate("RedirectLocation", null);
        }

        private void RedirectToAuthenticationPage(string currentRequestedTemplate = null)
        {
            switch (this._DomainControl.ServiceType)
            {
                case Basics.Domain.ServiceTypes.Template:
                    // Get AuthenticationPage 
                    KeyValuePair<string, string> referrerUrlQueryString = 
                        new KeyValuePair<string, string>();
                    string authenticationPage =
                        this._DomainControl.Domain.Settings.Configurations.AuthenticationTemplate;

                    if (!string.IsNullOrEmpty(currentRequestedTemplate) &&
                        string.Compare(authenticationPage, currentRequestedTemplate, StringComparison.OrdinalIgnoreCase) != 0)
                        referrerUrlQueryString =
                            new KeyValuePair<string, string>(
                                "xcRef",
                                System.Web.HttpUtility.UrlEncode(this.Context.Request.Header.Url.Raw.Substring(1))
                            );

                    // Reset Redirect Location to AuthenticationPage
                    this.Context.AddOrUpdate("RedirectLocation",
                        Helpers.CreateUrl(true, authenticationPage, referrerUrlQueryString));

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
            finally
            {
                writer?.Dispose();
            }

            this.WriteOutput(this._DomainControl.ServiceMimeType, writer.ToString(), this._SupportCompression);
        }

        private void CreateTemplateResult(Message messageResult, string methodResult)
        {
            string updateBlockControlId =
                this.Context.Request.Header["X-BlockRenderingId"];

            if (!string.IsNullOrEmpty(updateBlockControlId))
                this._DomainControl.RenderService(messageResult, updateBlockControlId.Split('>'));
            else
                this._DomainControl.RenderService(messageResult, null);

            if (this.Context.Response.Header.Status.Code == 200 && this._DomainControl.ServiceResult.HasErrors)
                this.Context.Response.Header.Status.Code = 218;

            StringBuilder sB = new StringBuilder();

            sB.Append(this._DomainControl.ServiceResult.Content);
            sB.Append(methodResult);

            string result = sB.ToString();
            const string sysRenderDurationMark = "<!--_sys_PAGERENDERDURATION-->";
            int idxRenderDurationMark =
                result.IndexOf(sysRenderDurationMark, StringComparison.InvariantCulture);
            if (idxRenderDurationMark > -1)
            {
                TimeSpan endRequestTimeSpan = DateTime.Now.Subtract(this._BeginRequestTime);

                result = result.Remove(idxRenderDurationMark, sysRenderDurationMark.Length);
                result = result.Insert(idxRenderDurationMark, endRequestTimeSpan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            }

            if (!this._DomainControl.IsWorkingAsStandAlone &&
                string.IsNullOrEmpty(updateBlockControlId))
            {
                StringWriter writer = null;
                try
                {
                    writer = new StringWriter();
                    this.CreateHtmlTag(ref writer, result);
                    writer.Flush();

                    result = writer.ToString();
                }
                finally
                {
                    writer?.Dispose();
                }
            }

            this.WriteOutput(this._DomainControl.ServiceMimeType, result, this._SupportCompression);
        }

        private void CreateHtmlTag(ref StringWriter writer, string bodyContent)
        {
            writer.WriteLine(
                Configurations.Xeora.Application.Main.UseHtml5Header
                    ? "<!doctype html>"
                    : "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">"
            );

            writer.WriteLine($"<html lang=\"{this._DomainControl.Domain.Languages.Current.Info.Id}\">");

            this.AppendHeadTag(ref writer);
            this.AppendBodyTag(ref writer, bodyContent);

            writer.WriteLine("</html>");
        }

        private void AppendHeadTag(ref StringWriter writer)
        {
            writer.WriteLine("<head>");

            this.AppendMetaTags(ref writer);

            writer.WriteLine(
                $"<title>{this._DomainControl.SiteTitle}</title>"
            );

            if (!string.IsNullOrEmpty(this._DomainControl.SiteIconUrl))
            {
                writer.WriteLine(
                    $"<link href=\"{this._DomainControl.SiteIconUrl}\" rel=\"shortcut icon\">"
                );
            }

            writer.WriteLine(
                $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{this._DomainControl.Domain.ContentsVirtualPath}/styles.css\" />"
            );

            writer.WriteLine(
                "<script type=\"text/javascript\" src=\"{0}_bi_sps_v{1}.js\"></script>", 
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation, 
                Application.DomainControl.XeoraJsVersion);
            writer.WriteLine(
                $"<script type=\"text/javascript\">__XeoraJS.pushCode({this.Context.HashCode});</script>"
            );

            writer.WriteLine("</head>");
        }

        private void AppendMetaTags(ref StringWriter writer)
        {
            bool isContentTypeAdded = false, isPragmaAdded = false, isCacheControlAdded = false, isExpiresAdded = false;

            foreach (var (key, value) in this._DomainControl.MetaRecord.CommonTags)
            {
                switch (Basics.MetaRecord.QueryTagSpace(key))
                {
                    case Basics.MetaRecord.TagSpaces.name:
                        writer.WriteLine(
                            $"<meta name=\"{Basics.MetaRecord.GetTagHtmlName(key)}\" content=\"{value}\" />"
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.httpequiv:
                        writer.WriteLine(
                            $"<meta http-equiv=\"{Basics.MetaRecord.GetTagHtmlName(key)}\" content=\"{value}\" />"
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.property:
                        writer.WriteLine(
                            $"<meta property=\"{Basics.MetaRecord.GetTagHtmlName(key)}\" content=\"{value}\" />"
                        );

                        break;
                }

                switch (key)
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

            foreach (var (key, value) in this._DomainControl.MetaRecord.CustomTags)
            {
                string keyName = key;
                switch (Basics.MetaRecord.QueryTagSpace(ref keyName))
                {
                    case Basics.MetaRecord.TagSpaces.name:
                        writer.WriteLine(
                            $"<meta name=\"{keyName}\" content=\"{value}\" />"
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.httpequiv:
                        writer.WriteLine(
                            $"<meta http-equiv=\"{keyName}\" content=\"{value}\" />"
                        );

                        break;
                    case Basics.MetaRecord.TagSpaces.property:
                        writer.WriteLine(
                            $"<meta property=\"{keyName}\" content=\"{value}\" />"
                        );

                        break;
                }
            }

            if (!isContentTypeAdded)
            {
                writer.WriteLine(
                    "<meta http-equiv=\"Content-Type\" content=\"{0}; charset={1}\" />", 
                    this._DomainControl.ServiceMimeType, Encoding.UTF8.WebName);
            }

            Basics.Enum.PageCachingTypes defaultType =
                this._DomainControl.Domain.Settings.Configurations.DefaultCaching;

            if (defaultType != Basics.Enum.PageCachingTypes.NoCache &&
                defaultType != Basics.Enum.PageCachingTypes.NoCacheCookieless) return;
            
            if (!isPragmaAdded)
                writer.WriteLine("<meta http-equiv=\"Pragma\" content=\"no-cache\" />");
            if (!isCacheControlAdded)
                writer.WriteLine("<meta http-equiv=\"Cache-Control\" content=\"no-cache\" />");
            if (!isExpiresAdded)
                writer.WriteLine("<meta http-equiv=\"Expires\" content=\"0\" />");
        }

        private void AppendBodyTag(ref StringWriter writer, string bodyContent)
        {
            writer.WriteLine("<body>");
            writer.WriteLine(
                "<form method=\"post\" action=\"{0}{1}/{2}?{3}\" enctype=\"multipart/form-data\" style=\"margin: 0px; padding: 0px;\">", 
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation, 
                this.Context.HashCode, 
                this._DomainControl.ServiceDefinition.FullPath, 
                this.Context.Request.Header.Url.QueryString
            );
            writer.WriteLine("<input type=\"hidden\" name=\"_sys_bind_{0}\" id=\"_sys_bind_{0}\" />", this.Context.HashCode);

            writer.Write(bodyContent);

            writer.WriteLine("</form>");
            writer.WriteLine("</body>");
        }

        private static void CompressUnsafe(ref Stream outputStream, out Stream gzippedStream)
        {
            byte[] contentBuffer = new byte[102400];

            GZipStream gzipCompression = null;
            try
            {
                gzippedStream = new MemoryStream();
                gzipCompression = new GZipStream(gzippedStream, CompressionMode.Compress, true);

                do
                {
                    int bC = outputStream.Read(contentBuffer, 0, contentBuffer.Length);
                    if (bC == 0) break;

                    gzipCompression.Write(contentBuffer, 0, bC);
                } while (true);
            }
            finally
            {
                gzipCompression?.Dispose();
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
            finally
            {
                outputStream?.Dispose();
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
                    Xeora.CompressUnsafe(ref outputStream, out gzippedStream);

                    if (gzippedStream != null && gzippedStream.Length < outputStream.Length)
                    {
                        this.Context.Response.Header.AddOrUpdate("Content-Encoding", "gzip");
                        this.WriteToSocketUnsafe(ref gzippedStream);

                        return;
                    }
                }
                finally
                {
                    gzippedStream?.Dispose();
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

            outputStream.Seek(0, SeekOrigin.Begin);
            do
            {
                int bC = outputStream.Read(contentBuffer, 0, contentBuffer.Length);
                if (bC == 0) break;
                
                this.Context.Response.Write(contentBuffer, 0, bC);

                if (applyBandwidthRules && bC == bandwidth)
                    Thread.Sleep(1000);
            } while (true);
        }
    }
}
