using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Context;

namespace Xeora.Web.Service
{
    public class ClientState
    {
        public static void Handle(IPAddress remoteAddr, Net.NetworkStream streamEnclosure)
        {
            string stateId = Guid.NewGuid().ToString();

            Basics.Context.IHttpContext context = null;
            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                Basics.Context.IHttpRequest request = new HttpRequest(remoteAddr);
                if (!((HttpRequest)request).Build(stateId, streamEnclosure))
                    return;

                context = new HttpContext(stateId, ref request);

                DateTime xeoraHandlerProcessBegins = DateTime.Now;

                IHandler xeoraHandler =
                    Handler.Manager.Current.Create(ref context);
                ((Handler.XeoraHandler)xeoraHandler).Handle();

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - xeora handler",
                        $"{DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds}ms", 
                        string.Empty, false);
                }

                DateTime responseFlushBegins = DateTime.Now;

                context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                context.Response.Header.AddOrUpdate("X-Powered-By", "Xeora");
                context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                context.Response.Header.AddOrUpdate("Connection", "close");

                ((HttpResponse)context.Response).Flush(streamEnclosure);

                Handler.Manager.Current.UnMark(xeoraHandler.HandlerId);

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - response flush",
                        $"{DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds}ms", 
                        string.Empty, false);

                    Basics.Console.Push(
                        "analytic - whole process",
                        $"{DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds}ms", 
                        string.Empty, false);
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
                    Basics.Console.Push("SYSTEM ERROR", string.Empty, ex.ToString(), false);

                ClientState.PushServerError(ref streamEnclosure);

                StatusTracker.Current.Increase(500);
            }
            finally
            {
                context?.Dispose();
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
