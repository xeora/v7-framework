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
        }
        
        public void Start()
        {
            if (this._Watcher == null) 
                this._Watcher = new Thread(this.Watch) {Priority = ThreadPriority.Lowest, IsBackground = true};
            if (this._Watcher.IsAlive) return;
            
            this.CreateWatchList(this._DomainRootLocation);
            this._Watcher.Start();
        }

        private void CreateWatchList(string domainRootPath)
        {
            DirectoryInfo domains =
                new DirectoryInfo(domainRootPath);
            if (!domains.Exists) return;

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
                    this.CreateWatchList(domainChildren.FullName);
            }
        }

        private void Watch()
        {
            bool checking = true;
            do
            {
                Thread.Sleep(Watcher.WATCHER_INTERVAL);
                
                foreach (FileInfo fileInfo in this._DomainExecutables)
                {
                    DateTime currentLastWrite =
                        fileInfo.LastWriteTimeUtc;
                    fileInfo.Refresh();
                    DateTime newLastWrite =
                        fileInfo.LastWriteTimeUtc;

                    if (DateTime.Compare(currentLastWrite, newLastWrite) == 0) continue;

                    Task.Factory.StartNew(() => this._WatcherCallback?.Invoke());
                    checking = false;
                    
                    break;
                }
            } while (checking);
            
            this._DomainExecutables.Clear();
            this._Watcher = null;
        }
    }
}
