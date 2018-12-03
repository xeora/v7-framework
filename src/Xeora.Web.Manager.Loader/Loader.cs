using System;
using System.IO;
using System.Threading;

namespace Xeora.Web.Manager
{
    internal class Loader
    {
        private static object _LoaderLock = new object();
        private static Loader _Current = null;

        private string _CacheRootLocation;
        private string _DomainRootLocation;
        private bool _LoadRequested;
        private FileSystemWatcher _FileSystemWatcher = null;

        private Action _ReloadedHandler;

        private Loader(Action reloadedHandler)
        {
            this._ReloadedHandler = reloadedHandler;

            this._CacheRootLocation =
                System.IO.Path.Combine(
                    Basics.Configurations.Xeora.Application.Main.TemporaryRoot,
                    Basics.Configurations.Xeora.Application.Main.WorkingPath.WorkingPathID
                );

            if (!Directory.Exists(this._CacheRootLocation))
                Directory.CreateDirectory(this._CacheRootLocation);

            this._DomainRootLocation =
                System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(
                        Basics.Configurations.Xeora.Application.Main.PhysicalRoot,
                        Basics.Configurations.Xeora.Application.Main.ApplicationRoot.FileSystemImplementation,
                        "Domains"
                    )
                );

            this._LoadRequested = false;

            this._FileSystemWatcher = new FileSystemWatcher();
            this._FileSystemWatcher.Path = this._DomainRootLocation;
            this._FileSystemWatcher.IncludeSubdirectories = true;
            this._FileSystemWatcher.Filter = "*.dll";
            this._FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            this._FileSystemWatcher.EnableRaisingEvents = true;
            this._FileSystemWatcher.Changed += (object sender, FileSystemEventArgs e) => Loader.Reload();

            this.LoadApplication();
        }

        public string ID { get; private set; }
        public string Path =>
            System.IO.Path.Combine(this._CacheRootLocation, this.ID);

        public static Loader Current => 
            Loader._Current;
        public static void Initialize(Action reloadedHandler)
        {
            Monitor.Enter(Loader._LoaderLock);
            try
            {
                if (Loader._Current != null)
                {
                    if (Loader._Current._LoadRequested)
                    {
                        Loader._Current.LoadApplication();
                        Loader._Current._LoadRequested = false;
                    }

                    return;
                }

                Loader._Current = new Loader(reloadedHandler);
            }
            finally
            {
                Monitor.Exit(Loader._LoaderLock);
            }
        }

        public static void Reload()
        {
            if (!Monitor.TryEnter(Loader._LoaderLock))
                return;

            if (Loader._Current == null)
                return;

            Loader._Current._LoadRequested = true;

            Monitor.Exit(Loader._LoaderLock);
        }

        private void LoadApplication()
        {
            try
            {
                this.ID = Guid.NewGuid().ToString();
                string applicationLocation =
                    System.IO.Path.Combine(this._CacheRootLocation, this.ID);
                if (!Directory.Exists(applicationLocation))
                    Directory.CreateDirectory(applicationLocation);

                this.LoadDomainExecutables(this._DomainRootLocation);

                Basics.Console.Push(string.Empty, "Application is loaded successfully!", string.Empty, false);

                this._ReloadedHandler?.Invoke();
            }
            catch (System.Exception)
            {
                Basics.Console.Push(string.Empty, "Application loading progress has been FAILED!", string.Empty, false);
            }

            // Do not block Application load
            ThreadPool.QueueUserWorkItem((object state) => this.Cleanup());
        }

        private void LoadDomainExecutables(string domainRootPath)
        {
            DirectoryInfo domainsDI =
                new DirectoryInfo(domainRootPath);

            foreach (DirectoryInfo domainDI in domainsDI.GetDirectories())
            {
                string domainExecutablesLocation =
                    System.IO.Path.Combine(domainDI.FullName, "Executables");

                DirectoryInfo domainExecutablesDI =
                    new DirectoryInfo(domainExecutablesLocation);
                if (domainExecutablesDI.Exists)
                {
                    foreach (FileInfo executableFI in domainExecutablesDI.GetFiles())
                    {
                        FileInfo applicationLocationFI =
                            new FileInfo(
                                System.IO.Path.Combine(this.Path, executableFI.Name));

                        if (applicationLocationFI.Exists)
                            continue;

                        try
                        {
                            executableFI.CopyTo(applicationLocationFI.FullName, true);
                        }
                        catch (System.Exception)
                        {
                            // Just Handle Exceptions
                        }
                    }
                }

                DirectoryInfo domainChildrenDI =
                    new DirectoryInfo(System.IO.Path.Combine(domainDI.FullName, "Addons"));
                if (domainChildrenDI.Exists)
                    this.LoadDomainExecutables(domainChildrenDI.FullName);
            }
        }

        private bool AvailableToDelete(DirectoryInfo applicationDI)
        {
            foreach (FileInfo fI in applicationDI.GetFiles())
            {
                Stream checkFS = null;
                try
                {
                    checkFS = fI.OpenRead();
                }
                catch (System.Exception)
                {
                    return false;
                }
                finally
                {
                    if (checkFS != null)
                        checkFS.Close();
                }
            }

            return true;
        }

        private void Cleanup()
        {
            DirectoryInfo cacheRootDI =
                new DirectoryInfo(this._CacheRootLocation);
            if (!cacheRootDI.Exists)
                return;

            foreach (DirectoryInfo applicationDI in cacheRootDI.GetDirectories())
            {
                if (applicationDI.Name.Equals("PoolSessions") ||
                    applicationDI.Name.Equals(this.ID))
                    continue;

                if (this.AvailableToDelete(applicationDI))
                {
                    try
                    {
                        applicationDI.Delete(true);
                    }
                    catch (System.Exception)
                    {
                        // Just Handle Exceptions
                    }
                }
            }

            Basics.Console.Push(string.Empty, "Cache is cleaned up!", string.Empty, false);
        }
    }
}
