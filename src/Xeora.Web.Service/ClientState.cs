using System;
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
                    if (!((HttpRequest) request).Build(stateId, streamEnclosure))
                        return;
                    
                    Basics.Context.IHttpResponse response = 
                        new HttpResponse(
                            stateId, 
                            string.Compare(
                                    request.Header["Connection"], 
                                    "keep-alive",
                                    StringComparison.OrdinalIgnoreCase) == 0,
                            header =>
                        {
                            header.AddOrUpdate("Server", "XeoraEngine");
                            header.AddOrUpdate("X-Powered-By", "Xeora");
                            header.AddOrUpdate("X-Framework-Version", Server.GetVersionText());
                        });
                    ((HttpResponse) response).StreamEnclosureRequested +=
                        (out NetworkStream enclosure) => enclosure = streamEnclosure;

                    ClientState.AcquireSession(request, out Basics.Session.IHttpSession session);
                    context =
                        new HttpContext(stateId, Configurations.Xeora.Service.Ssl, request, response, session, ApplicationContainer.Current);
                    PoolManager.KeepAlive(session.SessionId, context.HashCode);

                    DateTime xeoraHandlerProcessBegins = DateTime.Now;

                    xeoraHandler =
                        Handler.Manager.Current.Create(ref context);
                    ((Handler.Xeora) xeoraHandler).Handle();

                    if (Configurations.Xeora.Application.Main.PrintAnalysis)
                    {
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

                    DateTime responseFlushBegins = DateTime.Now;

                    ((HttpResponse) context.Response).Flush(streamEnclosure);

                    if (Configurations.Xeora.Application.Main.PrintAnalysis)
                    {
                        double totalMs =
                            DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds;
                        Basics.Console.Push(
                            "analysed - response flush",
                            $"{totalMs}ms",
                            string.Empty, false, groupId: stateId,
                            type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold
                                ? Basics.Console.Type.Warn
                                : Basics.Console.Type.Info);

                        totalMs = DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds;
                        Basics.Console.Push(
                            "analysed - whole process",
                            $"{totalMs}ms ({context.Request.Header.Url.Raw})",
                            string.Empty, false, groupId: stateId,
                            type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold
                                ? Basics.Console.Type.Warn
                                : Basics.Console.Type.Info);
                    }

                    StatusTracker.Current.Increase(context.Response.Header.Status.Code);
                }
                catch (Exception ex)
                {
                    // Skip SocketExceptions
                    if (ex is IOException && ex.InnerException is SocketException)
                        return;

                    Basics.Console.Push("Execution Exception...", ex.Message, ex.ToString(), false, true,
                        type: Basics.Console.Type.Error);

                    ClientState.PushServerError(ref streamEnclosure);

                    StatusTracker.Current.Increase(500);
                }
                finally
                {
                    if (xeoraHandler != null)
                        Handler.Manager.Current.Drop(xeoraHandler.HandlerId);
                    else
                        context?.Dispose();

                    Basics.Console.Flush(stateId);
                }
            } while (streamEnclosure.Alive());
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
        
        private static void PushServerError(ref Net.NetworkStream streamEnclosure)
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.Append("HTTP/1.1 500 Internal Server Error");
                sB.Append(HttpResponse.Newline);
                sB.Append("Connection: close");
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
