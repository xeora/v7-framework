using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xeora.Web.Configuration;

namespace Xeora.Web.Service
{
    public class WebServer
    {
        private string _ConfigurationPath;
        private string _ConfigurationFile;

        private TcpListener _TCPListener;
        private X509Certificate2 _Certificate;

        private const short READ_TIMEOUT = 5; // 5 seconds

        public WebServer(string configurationFilePath)
        {
            // Application Domain UnHandled Exception Event Handling Defination
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledExceptions);
            // !---

            Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelKeyPressed);

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


                    Basics.Console.Push("SSL Certificate Details", string.Empty, false);
                    Basics.Console.Push(" - Serial", this._Certificate.GetSerialNumberString(), false);
                    Basics.Console.Push(" - Issuer", this._Certificate.Issuer, false);
                    Basics.Console.Push(" - Subject", this._Certificate.Subject, false);
                    Basics.Console.Push(" - From", this._Certificate.GetEffectiveDateString(), false);
                    Basics.Console.Push(" - Till", this._Certificate.GetExpirationDateString(), false);
                    Basics.Console.Push(" - Format", this._Certificate.GetFormat(), false);
                    Basics.Console.Push(" - Public Key", this._Certificate.GetPublicKeyString(), false);
                }

                this._TCPListener = new TcpListener(serviceIPEndPoint);
                this._TCPListener.Start(100);

                Basics.Console.Push("XeoraEngine is started at", string.Format("{0} ({1})", serviceIPEndPoint, ConfigurationManager.Current.Configuration.Service.Ssl ? "Secure" : "Basic"), false);
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message = string.Format("{0} ({1})", message, ex.InnerException.Message);

				Basics.Console.Push("XeoraEngine is FAILED!", message, false, true);

                return 1;
            }

            Task connectionHandler =
                this.HandleConnections();
            connectionHandler.Wait();

            return 0;
        }

        private async Task HandleConnections()
        {
            while (true)
            {
                try
                {
                    TcpClient remoteClient =
                        await this._TCPListener.AcceptTcpClientAsync();
                    IPEndPoint remoteIPEndPoint = 
                        (IPEndPoint)remoteClient.Client.RemoteEndPoint;

                    this.EstablishConnection(remoteIPEndPoint, remoteClient);
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
                    Basics.Console.Push("Connection isn't established", ex.Message, false);

                    continue;
                }
            }
        }

        private async void EstablishConnection(IPEndPoint remoteIPEndPoint, TcpClient remoteClient)
        {
            Stream remoteStream = remoteClient.GetStream();

            if (ConfigurationManager.Current.Configuration.Service.Ssl)
            {
                remoteStream = new SslStream(remoteStream, false);

                try
                {
                    ((SslStream)remoteStream).AuthenticateAsServer(this._Certificate, false, System.Security.Authentication.SslProtocols.Tls12, true);
                }
                catch (IOException ex)
                {
                    Basics.Console.Push("Connection is rejected from", string.Format("{0} ({1})", remoteIPEndPoint, ex.Message), true);

                    remoteStream.Close();
                    remoteStream.Dispose();

                    return;
                }
                catch (System.Exception ex)
                {
                    Basics.Console.Push("Ssl Connection FAILED!", ex.ToString(), false, true);

                    remoteStream.Close();
                    remoteStream.Dispose();

                    return;
                }
            }

            // If reads create problems and put the loop to infinite. drop the connection.
            // that's why, 5 seconds timeout should be set to remoteStream
            // No need to put timeout to write operation because xeora will handle connection state
            remoteStream.ReadTimeout = READ_TIMEOUT * 1000;

            Net.NetworkStream streamEnclosure = new Net.NetworkStream(remoteStream);

            Basics.Console.Push("Connection is accepted from", string.Format("{0} ({1})", remoteIPEndPoint, ConfigurationManager.Current.Configuration.Service.Ssl ? "Secure" : "Basic"), true);

            ClientState clientState = new ClientState(remoteIPEndPoint.Address, streamEnclosure);
            await Task.Run(() => clientState.Handle());
            clientState.Dispose();

            streamEnclosure.Dispose();

            remoteClient.Close();
            remoteClient.Dispose();
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
        private void OnCancelKeyPressed(object source, ConsoleCancelEventArgs args)
        {
            if (this._Terminating)
                return;
            this._Terminating = true;

            Basics.Console.Push("Terminating XeoraEngine...", string.Empty, false);

            this._TCPListener.Stop();

            args.Cancel = true;
        }
    }
}