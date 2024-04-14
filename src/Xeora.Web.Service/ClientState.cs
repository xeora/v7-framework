using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Application;
using Xeora.Web.Service.Context;
using Xeora.Web.Service.Session;
using Xeora.Web.Service.VariablePool;
using NetworkStream = Xeora.Web.Service.Net.NetworkStream;

namespace Xeora.Web.Service
{
    public static class ClientState
    {
        public static void Handle(IPAddress remoteAddress, NetworkStream streamEnclosure)
        {
            do
            {
                string stateId = Guid.NewGuid().ToString();

                Basics.Context.IHttpContext context = null;
                IHandler xeoraHandler = null;
                try
                {
                    DateTime wholeProcessBegins = DateTime.Now;
                    
                    Basics.Context.IHttpRequest request = new HttpRequest(remoteAddress);

                    switch (((HttpRequest) request).Build(stateId, streamEnclosure))
                    {
                        case ParserResultTypes.Timeout:
                            return;
                        case ParserResultTypes.BadRequest:
                            ClientState.PushError(400, "Bad Request", ref streamEnclosure);
                            return;
                        case ParserResultTypes.MethodNotAllowed:
                            ClientState.PushError(405, "Method Not Allowed", ref streamEnclosure);
                            return;
                        case ParserResultTypes.HttpVersionNotSupported:
                            ClientState.PushError(505, "HTTP Version Not Supported", ref streamEnclosure);
                            return;
                    }
                    
                    if (((Context.Request.HttpRequestHeader)request.Header).WebSocket)
                    {
                        ParserResultTypes result =
                            ((HttpRequest)request).ExportAsWebSocket(out WebSocketRequest webSocketRequest);
                        switch (result)
                        {
                            case ParserResultTypes.Success:
                                // continue to process
                                break;
                            case ParserResultTypes.WebSocketVersionNotSupported:
                                ClientState.PushError(
                                    400, "Bad Request", ref streamEnclosure,
                                    new Dictionary<string, string> { { "Sec-WebSocket-Version", "13" } });
                                return;
                            default:
                                ClientState.PushError(400, "Bad Request", ref streamEnclosure);
                                return;
                        }
                        
                        ClientState.HandleWebSocketRequest(
                            stateId,
                            webSocketRequest,
                            streamEnclosure,
                            out xeoraHandler);
                        
                        continue;
                    }

                    ClientState.HandleHttpRequest(
                        stateId, 
                        wholeProcessBegins, 
                        request,
                        streamEnclosure,
                        out context,
                        out xeoraHandler);
                }
                catch (Exception ex)
                {
                    // Skip SocketExceptions
                    if (ex is IOException && ex.InnerException is SocketException)
                        return;

                    Basics.Console.Push("Execution Exception...", ex.Message, ex.ToString(), false, true,
                        type: Basics.Console.Type.Error);

                    ClientState.PushError(500, "Internal Server Error", ref streamEnclosure);

                    StatusTracker.Current.Increase(500);
                }
                finally
                {
                    if (xeoraHandler != null)
                    {
                        // Request have to be concluded before drop
                        ((HttpRequest)context?.Request)?.Conclude();
                        Handler.Manager.Current.Drop(xeoraHandler.HandlerId);
                    }
                    else
                        context?.Dispose();

                    Basics.Console.Flush(stateId);
                }
            } while (streamEnclosure.Alive());
        }

        private static void HandleHttpRequest(
            string stateId, 
            DateTime wholeProcessBegins, 
            Basics.Context.IHttpRequest request, 
            NetworkStream streamEnclosure,
            out Basics.Context.IHttpContext context,
            out IHandler xeoraHandler)
        {
            Basics.Context.IHttpResponse response =
                new HttpResponse(
                    stateId,
                    ((Context.Request.HttpRequestHeader)request.Header).KeepAlive,
                    header =>
                    {
                        header.AddOrUpdate("Server", "XeoraEngine");
                        header.AddOrUpdate("X-Powered-By", "Xeora");
                        header.AddOrUpdate("X-Framework-Version", Server.GetVersionText());
                    });
            ((HttpResponse)response).StreamEnclosureRequested +=
                (out NetworkStream enclosure) => enclosure = streamEnclosure;
            ((HttpResponse)response).ConcludeRequestRequested +=
                () => ((HttpRequest)request).Conclude();

            ClientState.AcquireSession(request, out Basics.Session.IHttpSession session);
            if (session == null) 
                throw new Exception("Unable to acquire session. Possibly DSS connectivity issue or session is expired");
            
            context =
                new HttpContext(stateId, Configurations.Xeora.Service.Ssl, request, response, session,
                    ApplicationContainer.Current);
            PoolManager.KeepAlive(session.SessionId, context.HashCode);

            DateTime xeoraHandlerProcessBegins = DateTime.Now;

            xeoraHandler =
                Handler.Manager.Current.Create(context);
            ((Handler.Xeora)xeoraHandler).Handle();

            ClientState.PrintHandlerAnalysis(stateId, xeoraHandlerProcessBegins);

            DateTime responseFlushBegins = DateTime.Now;

            ((HttpResponse)context.Response).Flush(streamEnclosure);

            ClientState.PrintResponseAnalysis(stateId, responseFlushBegins);
            ClientState.PrintWholeProcessAnalysis(stateId, wholeProcessBegins, context.Request.Header.Url.Raw);

            StatusTracker.Current.Increase(context.Response.Header.Status.Code);
        }

        private static void PrintHandlerAnalysis(string stateId, DateTime xeoraHandlerProcessBegins)
        {
            if (!Configurations.Xeora.Application.Main.PrintAnalysis) return;
            
            double totalMs =
                DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds;
            Basics.Console.Push(
                "analysed - xeora handler",
                $"{totalMs}ms",
                string.Empty, false, groupId: stateId,
                type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold
                    ? Basics.Console.Type.Warn
                    : Basics.Console.Type.Info);
        }

        private static void PrintResponseAnalysis(string stateId, DateTime responseFlushBegins)
        {
            if (!Configurations.Xeora.Application.Main.PrintAnalysis) return;
            
            double totalMs =
                DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds;
            Basics.Console.Push(
                "analysed - response flush",
                $"{totalMs}ms",
                string.Empty, false, groupId: stateId,
                type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold
                    ? Basics.Console.Type.Warn
                    : Basics.Console.Type.Info);
        }
        
        private static void PrintWholeProcessAnalysis(string stateId, DateTime wholeProcessBegins, string requestRawUrl)
        {
            if (!Configurations.Xeora.Application.Main.PrintAnalysis) return;

            double totalMs = 
                DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds;
            Basics.Console.Push(
                "analysed - whole process",
                $"{totalMs}ms ({requestRawUrl})",
                string.Empty, false, groupId: stateId,
                type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold
                    ? Basics.Console.Type.Warn
                    : Basics.Console.Type.Info);
        }
        
        private static void AcquireSession(Basics.Context.IHttpRequest request, out Basics.Session.IHttpSession session)
        {
            string sessionCookieKey = 
                Configurations.Xeora.Session.CookieKey;

            Basics.Context.IHttpCookieInfo sessionIdCookie =
                request.Header.Cookie[sessionCookieKey];
            request.Header.Cookie.Remove(sessionCookieKey);

            SessionManager.Current.Acquire(sessionIdCookie?.Value, out session);
        }
        
        private static void HandleWebSocketRequest(
            string stateId,
            Basics.Context.IWebSocketRequest webSocketRequest,
            NetworkStream streamEnclosure,
            out IHandler xeoraHandler)
        {
            Basics.Context.IWebSocketContext context =
                new WebSocketContext(stateId, webSocketRequest, Server.GetVersionText(), streamEnclosure);
            
            xeoraHandler =
                Handler.Manager.Current.Create(context);
            if (!((Handler.Xeora)xeoraHandler).Handle())
            {
                ClientState.PushError(400, "Bad Request", ref streamEnclosure);
                return;
            }

            ((WebSocketContext)context).Start();
        }

        private static void PushError(int code, string message, ref NetworkStream streamEnclosure, Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.AppendFormat("HTTP/1.1 {0} {1}", code, message);
                sB.Append(HttpResponse.Newline);
                if (additionalHeaders != null)
                {
                    foreach (var (key, value) in additionalHeaders)
                    {
                        sB.Append($"{key}: {value}");
                        sB.Append(HttpResponse.Newline);
                    }
                }
                sB.Append("Connection: close");
                sB.Append(HttpResponse.Newline);
                sB.Append(HttpResponse.Newline);

                byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
                streamEnclosure.Write(buffer, 0, buffer.Length);
                streamEnclosure.KeepAlive = false;
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
        }
    }
}
