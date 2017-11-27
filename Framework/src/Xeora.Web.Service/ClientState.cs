using System;
using System.IO;
using System.Net;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Service.Context;

namespace Xeora.Web.Service
{
    public class ClientState : IDisposable
    {
        private IPAddress _RemoteAddr;
        private Stream _InputStream;

        private Stream _ContentStream;

        private string _StateID;
        private string _TempLocation;

        private Basics.Context.IHttpRequest _Request = null;
        private Basics.Context.IHttpContext _Context = null;

        public ClientState(IPAddress remoteAddr, ref Stream inputStream)
        {
            this._RemoteAddr = remoteAddr;
            this._InputStream = inputStream;

            this._StateID = Guid.NewGuid().ToString();
            this._TempLocation = 
                Path.Combine(
                    Configuration.ConfigurationManager.Current.Configuration.Application.Main.TemporaryRoot, 
                    string.Format("ss-{0}.bin", this._StateID)
                );

            this._ContentStream = new FileStream(this._TempLocation, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            this._Request = new HttpRequest(this._RemoteAddr, ref this._ContentStream);
        }

        public void Handle()
        {
            byte[] buffer = new byte[524288];
            int bR = 0;

            try
            {
                DateTime wholeProcessBegins = DateTime.Now;

                do
                {
                    bR = this._InputStream.Read(buffer, 0, buffer.Length);

                    if (bR > 0)
                    {
                        this._ContentStream.Seek(0, SeekOrigin.End);
                        this._ContentStream.Write(buffer, 0, bR);

                        if (this.CreateContext())
                        {
                            DateTime xeoraHandlerProcessBegins = DateTime.Now;

                            IHandler xeoraHandler = 
                                Handler.HandlerManager.Current.Create(ref this._Context);
                            ((Handler.XeoraHandler)xeoraHandler).Handle();

                            if (Configurations.Xeora.Application.Main.PrintAnalytics)
                            {
                                Basics.Console.Push(
                                    "analytic - xeora handler",
                                    string.Format("{0}ms", DateTime.Now.Subtract(xeoraHandlerProcessBegins).Milliseconds), false);
                            }

                            DateTime responseFlushBegins = DateTime.Now;

                            this._Context.Response.Header.AddOrUpdate("Server", "XeoraEngine");
                            this._Context.Response.Header.AddOrUpdate("X-Powered-By", "XeoraCube");
                            this._Context.Response.Header.AddOrUpdate("X-Framework-Version", WebServer.GetVersionText());
                            this._Context.Response.Header.AddOrUpdate("Connection", "close");

                            ((HttpResponse)this._Context.Response).Flush(ref this._InputStream);

                            Handler.HandlerManager.Current.Mark(xeoraHandler.HandlerID);

                            if (Configurations.Xeora.Application.Main.PrintAnalytics)
                            {
                                Basics.Console.Push(
                                    "analytic - response flush",
                                    string.Format("{0}ms", DateTime.Now.Subtract(responseFlushBegins).Milliseconds), false);
                            }

                            break;
                        }

                        continue;
                    }

                    // Give some time to receive data.
                    System.Threading.Thread.Sleep(100);
                } while (true);

                if (Configurations.Xeora.Application.Main.PrintAnalytics)
                {
                    Basics.Console.Push(
                        "analytic - whole process",
                        string.Format("{0}ms", DateTime.Now.Subtract(wholeProcessBegins).Milliseconds), false);
                }
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is System.Net.Sockets.SocketException)
                    return;

                Helper.EventLogger.Log(ex);

                if (Configurations.Xeora.Service.Print)
                    Basics.Console.Push("SYSTEM ERROR", ex.ToString(), false);

                this.PushServerError();
            }
        }

        private bool CreateContext()
        {
            if (((HttpRequest)this._Request).Build(this._StateID))
            {
                this._Context = new HttpContext(this._StateID, ref this._Request);

                return true;
            }

            return false;
        }

        private void PushServerError()
        {
            try
            {
                StringBuilder sB = new StringBuilder();

                sB.AppendLine("HTTP/1.1 500 Internal Server Error");
                sB.AppendLine("Connection: close");

                byte[] buffer = Encoding.ASCII.GetBytes(sB.ToString());
                this._InputStream.Write(buffer, 0, buffer.Length);
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
            this._ContentStream.Close();

            File.Delete(this._TempLocation);
        }
    }
}
