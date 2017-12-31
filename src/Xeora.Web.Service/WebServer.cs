using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Xeora.Web.Configuration;

namespace Xeora.Web.Service
{
    public class WebServer
    {
        private string _ConfigurationPath;
        private string _ConfigurationFile;

        private TcpListener _tcpListener;

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

                this._tcpListener = new TcpListener(serviceIPEndPoint);
                this._tcpListener.Start(100);

                Basics.Console.Push("XeoraEngine is started at", serviceIPEndPoint.ToString(), false);
            }
            catch (System.Exception ex)
            {
                Basics.Console.Push("XeoraEngine is FAILED!", ex.Message, false, true);

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
                TcpClient remoteConnection = null;
                try
                {
                    remoteConnection =
                        await this._tcpListener.AcceptTcpClientAsync();

                    IPEndPoint remoteIPEndPoint = (IPEndPoint)remoteConnection.Client.RemoteEndPoint;
                    Basics.Console.Push("Connection is accepted from", remoteIPEndPoint.ToString(), true);
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

                HttpConnection httpConnection =
                    new HttpConnection(ref remoteConnection);
                httpConnection.HandleAsync();
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
        private void OnCancelKeyPressed(object source, ConsoleCancelEventArgs args)
        {
            if (this._Terminating)
                return;
            this._Terminating = true;

            Basics.Console.Push("Terminating XeoraEngine...", string.Empty, false);

            this._tcpListener.Stop();

            args.Cancel = true;
        }
    }
}