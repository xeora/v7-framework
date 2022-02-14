﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Xeora.Web.Service.VariablePool;
using Console = System.Console;
using Task = System.Threading.Tasks.Task;

namespace Xeora.Web.Service
{
    public class Server
    {
        private readonly Mutex _TerminationLock;
        private readonly string _ConfigurationPath;
        private readonly string _ConfigurationFile;
        private readonly string _Name;

        private TcpListener _TcpListener;
        private X509Certificate2 _Certificate;
        private SemaphoreSlim _SemaphoreSlim;

        public Server(string configurationFilePath, string name)
        {
            this._TerminationLock = new Mutex();
            
            // Application Domain UnHandled Exception Event Handling
            AppDomain.CurrentDomain.UnhandledException += Server.OnUnhandledExceptions;
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
            this._Name = name;
        }

        public async Task<int> StartAsync()
        {
            Server.PrintLogo();

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
                    string certPath = 
                        Path.Combine(this._ConfigurationPath, "server.p12");
                    if (!File.Exists(certPath))
                        throw new Exception("SSL certification file 'server.p12' is missing");

                    this._Certificate = new X509Certificate2(
                        certPath,
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

                    Basics.Console.Push("SSL Certificate Information", string.Empty, sslDetails.ToString(), false, true);
                }

                this._TcpListener = new TcpListener(serviceIpEndPoint);
                this._TcpListener.Start(100);

                Basics.Console.Push("XeoraEngine is started at", string.Format("{0} ({1}){2}", serviceIpEndPoint, Configuration.Manager.Current.Configuration.Service.Ssl ? "Secure" : "Basic", string.IsNullOrEmpty(this._Name) ? string.Empty: $" - {this._Name}"), string.Empty, false, true);
                
                PoolManager.Initialize(
                    Configuration.Manager.Current.Configuration.Session.Timeout);
                
                Negotiator negotiator = 
                    new Negotiator();
                typeof(Basics.Helpers).InvokeMember(
                    "Packet", 
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetProperty, 
                    null, 
                    null, 
                    new object[]{new Basics.DomainPacket("Default", negotiator)}
                );

                Manager.Loader.Initialize(
                    Configuration.Manager.Current.Configuration, 
                    (id, path) =>
                    {
                        Manager.Execution.ApplicationFactory.Initialize(negotiator, path);
                        Manager.Statement.Factory.Dispose();
                    });
                Manager.Execution.ApplicationFactory.Initialize(negotiator, Manager.Loader.Current.Path);
                
                ushort maxConnection = 
                    Basics.Configurations.Xeora.Service.Parallelism.MaxConnection;
                int workerThreads = 
                    Basics.Configurations.Xeora.Service.Parallelism.WorkerThreads;
                
                if (maxConnection > 0)
                {
                    Workers.Factory.Init(workerThreads);

                    this._SemaphoreSlim = new SemaphoreSlim(maxConnection);

                    Basics.Console.Push(
                        string.Empty,
                        $"Maximum simultaneous connection is limited to {maxConnection} with {workerThreads} WorkerThread(s)",
                        string.Empty, false, true);
                }
                else
                {
                    Workers.Factory.Init(workerThreads);

                    Basics.Console.Push(
                        string.Empty,
                        $"System currently working without any simultaneous connection limit with {workerThreads} WorkerThread(s)",
                        string.Empty, false, true);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message = $"{message} ({ex.InnerException.Message})";

				Basics.Console.Push("XeoraEngine is FAILED!", message, string.Empty, false, true, type: Basics.Console.Type.Error);

                return 1;
            }

            await this.ListenAsync();
            this._TerminationLock.WaitOne();
            
            return 0;
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    TcpClient remoteClient =
                        await this._TcpListener.AcceptTcpClientAsync();

                    if (this._SemaphoreSlim != null) await this._SemaphoreSlim.WaitAsync();

                    Workers.Factory.Queue(
                        c =>
                        {
                            ((Connection)c).Process();
                            this._SemaphoreSlim?.Release();
                        },
                        new Connection(ref remoteClient, this._Certificate)
                    );
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (SocketException)
                { /* Just Handle Exception */ }
                catch (Exception ex)
                {
                    Basics.Console.Push("Connection isn't established", ex.Message, string.Empty, false, true, type: Basics.Console.Type.Warn);
                }
            }
        }

        private static void PrintLogo()
        {
            Console.WriteLine();
            Console.WriteLine("____  ____                               ");
            Console.WriteLine("|_  _||_  _|                              ");
            Console.WriteLine("  \\ \\  / /  .---.   .--.   _ .--.  ,--.   ");
            Console.WriteLine("   > `' <  / /__\\\\/ .'`\\ \\[ `/'`\\]`'_\\ :  ");
            Console.WriteLine(" _/ /'`\\ \\_| \\__.,| \\__. | | |    // | |, ");
            Console.WriteLine("|____||____|'.__.' '.__.' [___]   \\'-;__/ ");

            Console.WriteLine();
            Console.WriteLine($"Web Development Framework, v{Server.GetVersionText()}");
            Console.WriteLine();
        }

        internal static string GetVersionText()
        {
            Version vI = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{vI.Major}.{vI.Minor}.{vI.Build}";
        }

        private static void OnUnhandledExceptions(object source, UnhandledExceptionEventArgs args)
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

            this._TerminationLock.WaitOne();
            try
            {
                if (args != null) args.Cancel = true;

                Basics.Console.Push(string.Empty, "Terminating XeoraEngine...", string.Empty, false, true);
                
                this._TcpListener?.Stop();

                Workers.Factory.Kill();
                
                // Terminate Loaded Domains
                Manager.Execution.ApplicationFactory.Terminate();
                
                Basics.Console.Flush().Wait();
            }
            finally {
                this._TerminationLock.ReleaseMutex();
            }
        }
    }
}