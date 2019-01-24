using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xeora.Web.Configuration;

namespace Xeora.Web.Service
{
    public class WebServer
    {
        private readonly string _ConfigurationPath;
        private readonly string _ConfigurationFile;

        private TcpListener _TcpListener;
        private X509Certificate2 _Certificate;

        public WebServer(string configurationFilePath)
        {
            // Application Domain UnHandled Exception Event Handling
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledExceptions;
            // !---

            // Application Domain SIGTERM Event Handling
            AppDomain.CurrentDomain.ProcessExit += (s, e) => this.OnTerminateSignal(s, null);
            // !---

            Console.CancelKeyPress += this.OnTerminateSignal;

            if (string.IsNullOrEmpty(configurationFilePath))
            {
                this._ConfigurationPath = Directory.GetCurrentDirectory();
                return;
            }

            this._ConfigurationPath = Path.GetDirectoryName(configurationFilePath);
            this._ConfigurationFile = Path.GetFileName(configurationFilePath);
        }

        public int Start()
        {
            this.PrintLogo();

            try
            {
                ConfigurationManager.Initialize(this._ConfigurationPath, this._ConfigurationFile);

                IPEndPoint serviceIPEndPoint =
                    new IPEndPoint(
                        ConfigurationManager.Current.Configuration.Service.Address,
                        ConfigurationManager.Current.Configuration.Service.Port
                    );

                if (ConfigurationManager.Current.Configuration.Service.Ssl)
                {
                    this._Certificate = new X509Certificate2(
                        Path.Combine(this._ConfigurationPath, "server.p12"),
                        ConfigurationManager.Current.Configuration.Service.CertificatePassword
                    );

                    System.Text.StringBuilder sslDetails = new System.Text.StringBuilder();
                    sslDetails.AppendFormat(" - Serial     {0}\n", this._Certificate.GetSerialNumberString());
                    sslDetails.AppendFormat(" - Issuer     {0}\n", this._Certificate.Issuer);
                    sslDetails.AppendFormat(" - Subject    {0}\n", this._Certificate.Subject);
                    sslDetails.AppendFormat(" - From       {0}\n", this._Certificate.GetEffectiveDateString());
                    sslDetails.AppendFormat(" - Till       {0}\n", this._Certificate.GetExpirationDateString());
                    sslDetails.AppendFormat(" - Format     {0}\n", this._Certificate.GetFormat());
                    sslDetails.AppendFormat(" - Public Key {0}", this._Certificate.GetPublicKeyString());

                    Basics.Console.Push("SSL Certificate Information", string.Empty, sslDetails.ToString(), false);
                }

                this._TcpListener = new TcpListener(serviceIPEndPoint);
                this._TcpListener.Start(100);

                Basics.Console.Push("XeoraEngine is started at", string.Format("{0} ({1})", serviceIPEndPoint, ConfigurationManager.Current.Configuration.Service.Ssl ? "Secure" : "Basic"), string.Empty, false);
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message = string.Format("{0} ({1})", message, ex.InnerException.Message);

				Basics.Console.Push("XeoraEngine is FAILED!", message, string.Empty, false, true);

                return 1;
            }

            this.Listen().GetAwaiter().GetResult();

            return 0;
        }

        private async Task Listen()
        {
            while (true)
            {
                try
                {
                    TcpClient remoteClient =
                        await this._TcpListener.AcceptTcpClientAsync();

                    Connection conn = new Connection(ref remoteClient, this._Certificate);
                    System.Threading.ThreadPool.QueueUserWorkItem(state => conn.Process());
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (SocketException)
                {
                    continue;
                }
                catch (System.Exception ex)
                {
                    Basics.Console.Push("Connection isn't established", ex.Message, string.Empty, false);

                    continue;
                }
            }
        }

        private void PrintLogo()
        {
            Console.WriteLine();
            Console.WriteLine("____  ____                               ");
            Console.WriteLine("|_  _||_  _|                              ");
            Console.WriteLine("  \\ \\  / /  .---.   .--.   _ .--.  ,--.   ");
            Console.WriteLine("   > `' <  / /__\\\\/ .'`\\ \\[ `/'`\\]`'_\\ :  ");
            Console.WriteLine(" _/ /'`\\ \\_| \\__.,| \\__. | | |    // | |, ");
            Console.WriteLine("|____||____|'.__.' '.__.' [___]   \\'-;__/ ");

            Console.WriteLine();
            Console.WriteLine(string.Format("Web Development Framework, v{0}", WebServer.GetVersionText()));
            Console.WriteLine();
        }

        internal static string GetVersionText()
        {
            Version vI = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}", vI.Major, vI.Minor, vI.Build);
        }

        private void OnUnhandledExceptions(object source, UnhandledExceptionEventArgs args)
        {
            if (args != null && args.ExceptionObject != null)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("----------- !!! --- Unhandled Exception --- !!! ------------");
                Console.WriteLine("------------------------------------------------------------");
                if (args.ExceptionObject is System.Exception)
                    Console.WriteLine(((System.Exception)args.ExceptionObject).ToString());
                else
                    Console.WriteLine(args.ExceptionObject.ToString());
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }

            Environment.Exit(500);
        }

        private bool _Terminating = false;
        private void OnTerminateSignal(object source, ConsoleCancelEventArgs args)
        {
            if (this._Terminating)
                return;
            this._Terminating = true;

            Basics.Console.Push("Terminating XeoraEngine...", string.Empty, string.Empty, false);

            this._TcpListener.Stop();

            if (args == null)
                return;

            args.Cancel = true;
        }
    }
}