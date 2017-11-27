using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Xeora.Web.Service
{
    public class WebServer
    {
        private TcpListener _tcpListener;

        public WebServer()
        {
            // Application Domain UnHandled Exception Event Handling Defination
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledExceptions);
            // !---

            Configuration.ConfigurationManager.Initialize(Directory.GetCurrentDirectory());

            IPEndPoint serviceIPEndPoint =
                new IPEndPoint(
                    Configuration.ConfigurationManager.Current.Configuration.Service.Address,
                    Configuration.ConfigurationManager.Current.Configuration.Service.Port
                );

            this._tcpListener = new TcpListener(serviceIPEndPoint);
        }

        public void Start()
        {
            this.PrintLogo();

            try
            {
                this._tcpListener.Start(100);

                IPEndPoint serviceIPEndPoint = (IPEndPoint)this._tcpListener.LocalEndpoint;
                Basics.Console.Push("XeoraEngine is started at", serviceIPEndPoint.ToString(), false);
            }
            catch (System.Exception ex)
            {
                Basics.Console.Push("XeoraEngine is FAILED!", ex.Message, false);

                Environment.Exit(1);
            }

            while (true)
            {
                TcpClient remoteConnection = null;
                try
                {
                    remoteConnection =
                        this._tcpListener.AcceptTcpClient();

                    IPEndPoint remoteIPEndPoint = (IPEndPoint)remoteConnection.Client.RemoteEndPoint;
                    Basics.Console.Push("Connection is accepted from", remoteIPEndPoint.ToString(), true);
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
                Console.WriteLine("----------- !!! --- Unhandled Exception --- !!! ------------");
                Console.WriteLine("------------------------------------------------------------");
                if (args.ExceptionObject is System.Exception)
                    Console.WriteLine(((System.Exception)args.ExceptionObject).ToString());
                else
                    Console.WriteLine(args.ExceptionObject.ToString());
                Console.WriteLine("------------------------------------------------------------");
            }

            Environment.Exit(500);
        }
    }
}
