using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xeora.Web.Basics
{
    public class Console
    {
        private readonly ConcurrentQueue<string> _Messages = null;
        private readonly ConcurrentDictionary<string, Action<ConsoleKeyInfo>> _KeyListeners = null;

        private Console()
        {
            this._Messages = new ConcurrentQueue<string>();
            this._KeyListeners = new ConcurrentDictionary<string, Action<ConsoleKeyInfo>>();

            ThreadPool.QueueUserWorkItem((state) => this.StartKeyListener());
        }

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

        private void StartKeyListener()
        {
            do
            {
                ConsoleKeyInfo keyInfo = 
                    System.Console.ReadKey(true);

                IEnumerator<KeyValuePair<string, Action<ConsoleKeyInfo>>> enumerator =
                    this._KeyListeners.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Action<ConsoleKeyInfo> action = 
                        enumerator.Current.Value;

                    ThreadPool.QueueUserWorkItem((state) => action.Invoke(keyInfo));
                }
            } while (true);
        }

        private string AddKeyListener(Action<ConsoleKeyInfo> callback)
        {
            if (callback == null)
                return Guid.Empty.ToString();

            string registrationId = Guid.NewGuid().ToString();

            if (!this._KeyListeners.TryAdd(registrationId, callback))
                return Guid.Empty.ToString();

            return registrationId;
        }

        private bool RemoveKeyListener(string callbackId)
        {
            if (string.IsNullOrEmpty(callbackId))
                return false;

            if (!this._KeyListeners.TryRemove(callbackId, out Action<ConsoleKeyInfo> value))
                return false;

            return true;
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
        public static void Push(string header, string summary, string details, bool applyRules, bool immediate = false)
        {
            if (applyRules && !Configurations.Xeora.Service.Print)
                return;

            if (string.IsNullOrEmpty(header))
                header = string.Empty;

            if (header.Length > 30)
                header = header.Substring(0, 30);

            header = header.PadRight(30, ' ');

            string consoleMessage = string.Format("{0} {1} {2}", DateTime.Now.ToString(), header, summary);
            if (!string.IsNullOrEmpty(details))
            {
                string detailsHeader = "--------------- Details ---------------";

                consoleMessage = string.Format("{0}\n\n{1}\n{2}\n\n", consoleMessage, detailsHeader, details);
            }

            if (immediate)
            {
                System.Console.WriteLine(consoleMessage);
                return;
            }

            Console.Current.Queue(consoleMessage);
        }

        /// <summary>
        /// Register an action to Xeora framework console key listener
        /// </summary>
        /// <returns>Registration ID</returns>
        /// <param name="callback">Listener action to be invoked when a key pressed on Xeora framework console</param>
        public static string Register(Action<ConsoleKeyInfo> callback) =>
            Console.Current.AddKeyListener(callback);

        /// <summary>
        /// Unregister an action registered with an ID previously
        /// </summary>
        /// <returns>Removal Result, <c>true</c> if removed; otherwise, <c>false</c></returns>
        /// <param name="registrationId">registration Id of action</param>
        public static bool Unregister(string registrationId) =>
            Console.Current.RemoveKeyListener(registrationId);
    }
}
