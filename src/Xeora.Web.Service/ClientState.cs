using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Context;

namespace Xeora.Web.Service
{
    public class ClientState : IDisposable
    {
        private IPAddress _RemoteAddr;
        private Net.NetworkStream _StreamEnclosure;

        private string _StateID;

        private Basics.Context.IHttpContext _Context = null;

        public ClientState(IPAddress remoteAddr, Net.NetworkStream streamEnclosure)
        {
            this._RemoteAddr = remoteAddr;
            this._StreamEnclosure = streamEnclosure;

            this._StateID = Guid.NewGuid().ToString();
        }

        public void Handle()
        {
            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                Basics.Context.IHttpRequest request = new HttpRequest(this._RemoteAddr);
                if (!((HttpRequest)request).Build(this._StateID, this._StreamEnclosure))
                    return;

                this._Context = new HttpContext(this._StateID, ref request);

                DateTime xeoraHandlerProcessBegins = DateTime.Now;

                IHandler xeoraHandler =
                    Handler.HandlerManager.Current.Create(ref this._Context);
                ((Handler.XeoraHandler)xeoraHandler).Handle();

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - xeora handler",
                        string.Format("{0}ms", DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds), 
                        string.Empty, false);
                }

                DateTime responseFlushBegins = DateTime.Now;

                this._Context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                this._Context.Response.Header.AddOrUpdate("X-Powered-By", "XeoraCube");
                this._Context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                this._Context.Response.Header.AddOrUpdate("Connection", "close");

                ((HttpResponse)this._Context.Response).Flush(this._StreamEnclosure);

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

                StatusTracker.Current.Increase(this._Context.Response.Header.Status.Code);
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Helper.EventLogger.Log(ex);

                if (Configurations.Xeora.Service.Print)
                    Basics.Console.Push("SYSTEM ERROR", string.Empty, ex.ToString(), false);

                this.PushServerError();

                StatusTracker.Current.Increase(500);
            }
        }

        private void PushServerError()
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.AppendLine("HTTP/1.1 500 Internal Server Error");
                sB.AppendLine("Connection: close");

                byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
                this._StreamEnclosure.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }
        }

        public void Dispose()
        {
            if (this._Context != null)
                this._Context.Dispose();
        }
    }
}
