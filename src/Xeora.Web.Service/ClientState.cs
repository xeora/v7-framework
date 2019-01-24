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
            string stateID = Guid.NewGuid().ToString();

            Basics.Context.IHttpContext context = null;
            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                Basics.Context.IHttpRequest request = new HttpRequest(remoteAddr);
                if (!((HttpRequest)request).Build(stateID, streamEnclosure))
                    return;

                context = new HttpContext(stateID, ref request);

                DateTime xeoraHandlerProcessBegins = DateTime.Now;

                IHandler xeoraHandler =
                    Handler.HandlerManager.Current.Create(ref context);
                ((Handler.XeoraHandler)xeoraHandler).Handle();

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - xeora handler",
                        string.Format("{0}ms", DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds), 
                        string.Empty, false);
                }

                DateTime responseFlushBegins = DateTime.Now;

                context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                context.Response.Header.AddOrUpdate("X-Powered-By", "XeoraCube");
                context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                context.Response.Header.AddOrUpdate("Connection", "close");

                ((HttpResponse)context.Response).Flush(streamEnclosure);

                Handler.HandlerManager.Current.UnMark(xeoraHandler.HandlerID);

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - response flush",
                        string.Format("{0}ms", DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds), 
                        string.Empty, false);

                    Basics.Console.Push(
                        "analytic - whole process",
                        string.Format("{0}ms", DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds), 
                        string.Empty, false);
                }

                StatusTracker.Current.Increase(context.Response.Header.Status.Code);
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Helper.EventLogger.Log(ex);

                if (Configurations.Xeora.Service.Print)
                    Basics.Console.Push("SYSTEM ERROR", string.Empty, ex.ToString(), false);

                ClientState.PushServerError(ref streamEnclosure);

                StatusTracker.Current.Increase(500);
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        private static void PushServerError(ref Net.NetworkStream streamEnclosure)
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.Append("HTTP/1.1 500 Internal Server Error");
                sB.Append(HttpResponse.NEWLINE);
                sB.Append("Connection: close");
                sB.Append(HttpResponse.NEWLINE);

                byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
                streamEnclosure.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }
        }
    }
}
