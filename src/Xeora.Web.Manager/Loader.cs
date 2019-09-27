using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xeora.Web.Manager
{
    internal class Loader
    {
        private readonly string _CacheRootLocation;
        private readonly string _DomainRootLocation;

        private Loader()
        {
            this._CacheRootLocation =
                System.IO.Path.Combine(
                    Basics.Configurations.Xeora.Application.Main.TemporaryRoot,
                    Basics.Configurations.Xeora.Application.Main.WorkingPath.WorkingPathId
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
            
            Watcher watcher = 
                new Watcher(this._DomainRootLocation, Basics.Helpers.Refresh);
            watcher.Start();
        }

        public string Id { get; private set; }
        public string Path =>
            System.IO.Path.Combine(this._CacheRootLocation, this.Id);

        public static Loader Current { get; private set; }
        public static void Initialize()
        {
            Loader.Current = new Loader();
            Loader.Current.Load();
        }

        public static void Reload()
        {
            if (Loader.Current == null) return;
            
            StatementFactory.Dispose();
            Application.Dispose();
            
            Loader.Current.Load();
        }

        private void Load()
        {
            try
            {
                this.Id = Guid.NewGuid().ToString();
                string applicationLocation =
                    System.IO.Path.Combine(this._CacheRootLocation, this.Id);
                if (!Directory.Exists(applicationLocation))
                    Directory.CreateDirectory(applicationLocation);

                this.LoadExecutables(this._DomainRootLocation);

                Basics.Console.Push(string.Empty, "Application is loaded successfully!", string.Empty, false);
            }
            catch (Exception)
            {
                Basics.Console.Push(string.Empty, "Application loading progress has been FAILED!", string.Empty, false, true, type: Basics.Console.Type.Error);
            }

            // Do not block Application load
            ThreadPool.QueueUserWorkItem(state => this.Cleanup());
        }

        private void LoadExecutables(string domainRootPath)
        {
            DirectoryInfo domains =
                new DirectoryInfo(domainRootPath);

            foreach (DirectoryInfo domain in domains.GetDirectories())
            {
                string domainExecutablesLocation =
                    System.IO.Path.Combine(domain.FullName, "Executables");

                DirectoryInfo domainExecutables =
                    new DirectoryInfo(domainExecutablesLocation);
                if (domainExecutables.Exists)
                {
                    foreach (FileInfo executable in domainExecutables.GetFiles())
                    {
                        FileInfo applicationLocation =
                            new FileInfo(
                                System.IO.Path.Combine(this.Path, executable.Name));

                        if (applicationLocation.Exists) continue;

                        executable.CopyTo(applicationLocation.FullName, true);
                    }
                }

                DirectoryInfo domainChildren =
                    new DirectoryInfo(System.IO.Path.Combine(domain.FullName, "Addons"));
                if (domainChildren.Exists)
                    this.LoadExecutables(domainChildren.FullName);
            }
        }

        private bool AvailableToDelete(DirectoryInfo application)
        {
            foreach (FileInfo fI in application.GetFiles())
            {
                Stream checkStream = null;
                try
                {
                    checkStream = fI.OpenRead();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    checkStream?.Close();
                }
            }

            return true;
        }

        private void Cleanup()
        {
            DirectoryInfo cacheRoot =
                new DirectoryInfo(this._CacheRootLocation);
            if (!cacheRoot.Exists)
                return;

            foreach (DirectoryInfo application in cacheRoot.GetDirectories())
            {
                if (application.Name.Equals("PoolSessions") ||
                    application.Name.Equals(this.Id))
                    continue;

                if (!this.AvailableToDelete(application)) continue;
                
                try
                {
                    application.Delete(true);
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
                }
            }

            Basics.Console.Push(string.Empty, "Cache is cleaned up!", string.Empty, false);
        }
    }
}
