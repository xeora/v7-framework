using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xeora.Web.Manager
{
    internal class Watcher
    {
        private const int WATCHER_INTERVAL = 5 * 1000; // 5 Seconds;
        
        private readonly string _DomainRootLocation;
        private readonly List<FileInfo> _DomainExecutables;
        private Thread _Watcher;
        private readonly Action _WatcherCallback;

        public Watcher(string domainRootLocation, Action watcherCallback)
        {
            this._DomainRootLocation = domainRootLocation;
            this._WatcherCallback = watcherCallback;
            this._DomainExecutables = new List<FileInfo>();
            
            this.Create();
        }
        
        private void Create() =>
            this._Watcher = new Thread(this.Watch) {Priority = ThreadPriority.Lowest, IsBackground = true};

        public void Start()
        {
            if (this._Watcher.IsAlive) return;
            
            this.LoadExecutables(this._DomainRootLocation);
            this._Watcher.Start();
        }

        public void Stop()
        {
            this._Watcher.Abort();
            this.Create();
            
            this._DomainExecutables.Clear();
        }
        
        private void LoadExecutables(string domainRootPath)
        {
            DirectoryInfo domains =
                new DirectoryInfo(domainRootPath);

            foreach (DirectoryInfo domain in domains.GetDirectories())
            {
                string domainExecutablesLocation =
                    Path.Combine(domain.FullName, "Executables");

                DirectoryInfo domainExecutables =
                    new DirectoryInfo(domainExecutablesLocation);
                if (domainExecutables.Exists)
                {
                    foreach (FileInfo executable in domainExecutables.GetFiles())
                    {
                        if (string.CompareOrdinal(executable.Extension.ToLower(), ".dll") != 0) continue;
                        this._DomainExecutables.Add(executable);
                    }
                }

                DirectoryInfo domainChildren =
                    new DirectoryInfo(Path.Combine(domain.FullName, "Addons"));
                if (domainChildren.Exists)
                    this.LoadExecutables(domainChildren.FullName);
            }
        }

        private void Watch()
        {
            do
            {
                foreach (FileInfo fileInfo in this._DomainExecutables)
                {
                    DateTime currentLastWrite =
                        fileInfo.LastWriteTimeUtc;
                    fileInfo.Refresh();
                    DateTime newLastWrite =
                        fileInfo.LastWriteTimeUtc;

                    if (DateTime.Compare(currentLastWrite, newLastWrite) == 0) continue;

                    Task.Factory.StartNew(() => this._WatcherCallback?.Invoke());
                    break;
                }
                
                Thread.Sleep(Watcher.WATCHER_INTERVAL);
            } while (true);
        }
    }
}
