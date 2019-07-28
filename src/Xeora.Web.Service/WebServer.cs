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
                Configuration.Manager.Initialize(this._ConfigurationPath, this._ConfigurationFile);

                IPEndPoint serviceIpEndPoint =
                    new IPEndPoint(
                        Configuration.Manager.Current.Configuration.Service.Address,
                        Configuration.Manager.Current.Configuration.Service.Port
                    );

                if (Configuration.Manager.Current.Configuration.Service.Ssl)
                {
                    this._Certificate = new X509Certificate2(
                        Path.Combine(this._ConfigurationPath, "server.p12"),
                        Configuration.Manager.Current.Configuration.Service.CertificatePassword
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

                this._TcpListener = new TcpListener(serviceIpEndPoint);
                this._TcpListener.Start(100);

                Basics.Console.Push("XeoraEngine is started at", string.Format("{0} ({1})", serviceIpEndPoint, Configuration.Manager.Current.Configuration.Service.Ssl ? "Secure" : "Basic"), string.Empty, false);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message = $"{message} ({ex.InnerException.Message})";

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

                    Task connectionTask =
                        new Task((c) => ((Connection)c).Process(),
                        new Connection(ref remoteClient, this._Certificate),
                        TaskCreationOptions.DenyChildAttach
                    );
                    connectionTask.Start();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (SocketException)
                { /* Just Handle Exception */ }
                catch (Exception ex)
                {
                    Basics.Console.Push("Connection isn't established", ex.Message, string.Empty, false);
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
            Console.WriteLine($"Web Development Framework, v{WebServer.GetVersionText()}");
            Console.WriteLine();
        }

        internal static string GetVersionText()
        {
            Version vI = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{vI.Major}.{vI.Minor}.{vI.Build}";
        }

        private void OnUnhandledExceptions(object source, UnhandledExceptionEventArgs args)
        {
            if (args?.ExceptionObject != null)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("----------- !!! --- Unhandled Exception --- !!! ------------");
                Console.WriteLine("------------------------------------------------------------");
                if (args.ExceptionObject is Exception exception)
                    Console.WriteLine(exception.ToString());
                else
                    Console.WriteLine(args.ExceptionObject.ToString());
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }

            Environment.Exit(500);
        }

        private bool _Terminating;
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