using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Xeora.Web.Basics
{
    public class Console
    {
        private ConcurrentQueue<string> _Messages = null;

        private Console() =>
            this._Messages = new ConcurrentQueue<string>();

        private void Queue(string message)
        {
            this._Messages.Enqueue(message);
            this.Flush();
        }

        private bool _Flushing = false;
        private async void Flush()
        {
            if (this._Flushing)
                return;
            this._Flushing = true;

            await Task.Run(() =>
            {
                while (!this._Messages.IsEmpty)
                {
                    string consoleMessage;
                    this._Messages.TryDequeue(out consoleMessage);

                    System.Console.WriteLine(consoleMessage);
                }

                this._Flushing = false;
            });
        }

        private static object _Lock = new object();
        private static Console _Current = null;
        private static Console Current
        {
            get
            {
                Monitor.Enter(Console._Lock);
                try
                {
                    if (Console._Current == null)
                        Console._Current = new Console();
                }
                finally
                {
                    Monitor.Exit(Console._Lock);
                }

                return Console._Current;
            }
        }

        /// <summary>
        /// Push the message to the Xeora framework console
        /// </summary>
        /// <param name="header">Message Title</param>
        /// <param name="message">Message Content</param>
        /// <param name="applyRules">If set to <c>true</c> obey the rules defined in Xeora project settings json</param>
        /// <param name="immediate">If set to <c>true</c> message will not be queued and print to the console immidiately</param>
        public static void Push(string header, string message, bool applyRules, bool immediate = false)
        {
            if (applyRules && !Configurations.Xeora.Service.Print)
                return;

            if (string.IsNullOrEmpty(header))
                header = string.Empty;

            if (header.Length > 30)
                header = header.Substring(0, 30);

            header = header.PadRight(30, ' ');

            string consoleMessage = string.Format("{0} {1} {2}", DateTime.Now.ToString(), header, message);

            if (immediate)
            {
                System.Console.WriteLine(consoleMessage);
                return;
            }

            Console.Current.Queue(consoleMessage);
        }
    }
}
