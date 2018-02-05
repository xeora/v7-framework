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
        private NetworkStream _RemoteStream;

        private string _StateID;

        private Basics.Context.IHttpRequest _Request = null;
        private Basics.Context.IHttpContext _Context = null;

        public ClientState(IPAddress remoteAddr, ref NetworkStream remoteStream)
        {
            this._RemoteAddr = remoteAddr;
            this._RemoteStream = remoteStream;

            this._StateID = Guid.NewGuid().ToString();
            this._Request = new HttpRequest(this._RemoteAddr);
        }

        public void Handle()
        {
            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                ((HttpRequest)this._Request).Build(this._StateID, ref this._RemoteStream);
                this._Context = new HttpContext(this._StateID, ref this._Request);

                DateTime xeoraHandlerProcessBegins = DateTime.Now;

                IHandler xeoraHandler =
                    Handler.HandlerManager.Current.Create(ref this._Context);
                ((Handler.XeoraHandler)xeoraHandler).Handle();

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - xeora handler",
                        string.Format("{0}ms", DateTime.Now.Subtract(xeoraHandlerProcessBegins).TotalMilliseconds), false);
                }

                DateTime responseFlushBegins = DateTime.Now;

                this._Context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                this._Context.Response.Header.AddOrUpdate("X-Powered-By", "XeoraCube");
                this._Context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                this._Context.Response.Header.AddOrUpdate("Connection", "close");

                ((HttpResponse)this._Context.Response).Flush(ref this._RemoteStream);

                Handler.HandlerManager.Current.UnMark(xeoraHandler.HandlerID);

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - response flush",
                        string.Format("{0}ms", DateTime.Now.Subtract(responseFlushBegins).TotalMilliseconds), false);
                }

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - whole process",
                        string.Format("{0}ms", DateTime.Now.Subtract(wholeProcessBegins).TotalMilliseconds), false);
                }
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Helper.EventLogger.Log(ex);

                if (Configurations.Xeora.Service.Print)
                    Basics.Console.Push("SYSTEM ERROR", ex.ToString(), false);

                this.PushServerError();
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
                this._RemoteStream.Write(buffer, 0, buffer.Length);
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
