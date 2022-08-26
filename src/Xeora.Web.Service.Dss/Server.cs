using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Xeora.Web.Service.Dss
{
    public class Server
    {
        private readonly Mutex _TerminationLock;
        private readonly ConcurrentDictionary<Guid, TcpClient> _Clients;
        private readonly Thread _ClientCleanupThread;
        
        private readonly IPEndPoint _ServiceEndPoint;
        private TcpListener _TcpListener;
        private readonly IManager _Manager;

        public Server(IPEndPoint serviceEndPoint)
        {
            this._TerminationLock = new Mutex();
            this._Clients = new ConcurrentDictionary<Guid, TcpClient>();
            this._ClientCleanupThread = new Thread(() =>
            {
                Basics.Console.Push(string.Empty, "Started client cleanup thread...", string.Empty, false, true);
                try
                {
                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromHours(1));

                        int purged = 0;
                        foreach (Guid key in this._Clients.Keys)
                        {
                            if (!this._Clients.TryGetValue(key, out TcpClient client)) continue;
                            if (client.Connected) continue;
                            
                            this._Clients.TryRemove(key, out _);
                            purged++;
                        }

                        if (purged == 0) continue;
                        Basics.Console.Push(string.Empty, $"Purged {purged} client(s)", string.Empty, false, true);
                    }
                }
                catch 
                { /* Just Handle Exceptions */ }
            })
            {
                IsBackground = true, 
                Priority = ThreadPriority.Lowest
            };

            // Application Domain UnHandled Exception Event Handling
            AppDomain.CurrentDomain.UnhandledException += Server.OnUnhandledExceptions;
            // !---

            // Application Domain SIGTERM Event Handling
            AppDomain.CurrentDomain.ProcessExit += (s, e) => this.OnTerminateSignal(s, null);
            // !---
            
            Console.CancelKeyPress += this.OnTerminateSignal;

            this._ServiceEndPoint = serviceEndPoint;
            this._Manager = new Internal.Manager();
        }

        public async Task<int> StartAsync()
        {
            Server.PrintLogo();

            try
            {
                this._TcpListener = new TcpListener(this._ServiceEndPoint);
                this._TcpListener.Start(100);

                Basics.Console.Push("XeoraDss is started at", this._ServiceEndPoint.ToString(), string.Empty, false, true);
                
                this._ClientCleanupThread.Start();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message = $"{message} ({ex.InnerException.Message})";
                
                Basics.Console.Push("XeoraDss is FAILED!", message, string.Empty, false, true, type: Basics.Console.Type.Error);

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
                    
                    this._Clients.TryAdd(Guid.NewGuid(), remoteClient);
                    
                    ThreadPool.QueueUserWorkItem(
                        c => ((Connection) c)?.Process(),
                        new Connection(ref remoteClient, this._Manager)
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
            Console.WriteLine($"Data Structure Storage Service, v{Server.GetVersionText()}");
            Console.WriteLine();
        }

        private static string GetVersionText()
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

                Basics.Console.Push(string.Empty, "Terminating XeoraDss...", string.Empty, false, true);
                
                this._TcpListener?.Stop();
                
                this._ClientCleanupThread.Interrupt();

                // Kill all connections (if any applicable)
                Basics.Console.Push(string.Empty, "Killing connected clients...", string.Empty, false, true);
                foreach (Guid key in this._Clients.Keys)
                {
                    this._Clients.TryRemove(key, out TcpClient client);
                    client?.Dispose();
                }

                Basics.Console.Flush().Wait();
            }
            finally {
                this._TerminationLock.ReleaseMutex();
            }
        }
    }
}
