using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace Xeora.Web.External.Service.Session
{
    public class SessionServer
    {
        private IPEndPoint _ServiceEndPoint;
        private TcpListener _tcpListener;

        public SessionServer(IPEndPoint serviceEndPoint)
        {
            // Application Domain UnHandled Exception Event Handling Defination
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledExceptions);
            // !---

            Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelKeyPressed);

            this._ServiceEndPoint = serviceEndPoint;
        }

        public int Start()
        {
            this.PrintLogo();

            try
            {
                this._tcpListener = new TcpListener(this._ServiceEndPoint);
                this._tcpListener.Start(100);

                Basics.Console.Push("Service is started at", this._ServiceEndPoint.ToString(), false);
            }
            catch (System.Exception ex)
            {
                Basics.Console.Push("Service is FAILED!", ex.Message, false, true);

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

                ConnectionHandler connectionHandler =
                    new ConnectionHandler(ref remoteConnection);
                connectionHandler.HandleAsync();
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
            Console.WriteLine(string.Format("Session Service, v{0}", SessionServer.GetVersionText()));
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

            Basics.Console.Push("Terminating Session Service...", string.Empty, false);

            this._tcpListener.Stop();

            args.Cancel = true;
        }
    }
}
