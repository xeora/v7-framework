using System;
using System.Collections.Concurrent;
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
        async private void Flush()
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

        private static Console _Instance = null;
        private static Console Instance
        {
            get
            {
                if (Console._Instance == null)
                    Console._Instance = new Console();

                return Console._Instance;
            }
        }

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

            Console.Instance.Queue(consoleMessage);
        }
    }
}
