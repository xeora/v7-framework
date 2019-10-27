using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Context;

namespace Xeora.Web.Service
{
    public static class ClientState
    {
        public static void Handle(IPAddress remoteAddr, Net.NetworkStream streamEnclosure)
        {
            string stateId = Guid.NewGuid().ToString();

            Basics.Context.IHttpContext context = null;
            IHandler xeoraHandler = null;
            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                Basics.Context.IHttpRequest request = new HttpRequest(remoteAddr);
                if (!((HttpRequest)request).Build(stateId, streamEnclosure))
                    return;

                context = new HttpContext(stateId, ref request);

                DateTime xeoraHandlerProcessBegins = DateTime.Now;

                xeoraHandler =
                    Handler.Manager.Current.Create(ref context);
                ((Handler.XeoraHandler)xeoraHandler).Handle();

                if (Configurations.Xeora.Application.Main.PrintAnalysis)
                {
                    double totalMs =
                        DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds;
                    Basics.Console.Push(
                        "analysed - xeora handler",
                        $"{totalMs}ms", 
                        string.Empty, false, groupId: stateId,
                        type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);
                }

                DateTime responseFlushBegins = DateTime.Now;

                context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                context.Response.Header.AddOrUpdate("X-Powered-By", "Xeora");
                context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                context.Response.Header.AddOrUpdate("Connection", "close");

                ((HttpResponse)context.Response).Flush(streamEnclosure);

                if (Configurations.Xeora.Application.Main.PrintAnalysis)
                {
                    double totalMs =
                        DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds;
                    Basics.Console.Push(
                        "analysed - response flush",
                        $"{totalMs}ms", 
                        string.Empty, false, groupId: stateId,
                        type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);

                    totalMs = DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds;
                    Basics.Console.Push(
                        "analysed - whole process",
                        $"{totalMs}ms ({context.Request.Header.Url.Raw})", 
                        string.Empty, false, groupId: stateId,
                        type: totalMs > Configurations.Xeora.Application.Main.AnalysisThreshold ? Basics.Console.Type.Warn: Basics.Console.Type.Info);
                }

                StatusTracker.Current.Increase(context.Response.Header.Status.Code);
            }
            catch (Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Tools.EventLogger.Log(ex);

                if (Configurations.Xeora.Service.Print)
                    Basics.Console.Push("SYSTEM ERROR", string.Empty, ex.ToString(), false, true, type: Basics.Console.Type.Error);

                ClientState.PushServerError(ref streamEnclosure);

                StatusTracker.Current.Increase(500);
            }
            finally
            {
                if (xeoraHandler != null)
                    Handler.Manager.Current.UnMark(xeoraHandler.HandlerId);
                
                context?.Dispose();
                
                Basics.Console.Flush(stateId);
            }
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
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
        }
    }
}
