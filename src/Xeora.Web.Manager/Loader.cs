using System;
using System.IO;
using System.Threading;

namespace Xeora.Web.Manager
{
    internal class Loader
    {
        private readonly string _CacheRootLocation;
        private readonly string _DomainRootLocation;
        // private readonly FileSystemWatcher _FileSystemWatcher = null;

        private Loader(Action libraryChanged)
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


            /* TODO: Canceled until we find a stabil solution for binary refreshing
            this._FileSystemWatcher = new FileSystemWatcher
            {
                Path = this._DomainRootLocation,
                IncludeSubdirectories = true,
                Filter = "*.dll",
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            this._FileSystemWatcher.Changed += (object sender, FileSystemEventArgs e) => this.Load(libraryChanged);

            Basics.Console.Register((keyInfo) => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.R)
                    return;

                this.Load(libraryChanged);
            });
            */
        }

        public string Id { get; private set; }
        public string Path =>
            System.IO.Path.Combine(this._CacheRootLocation, this.Id);

        public static Loader Current { get; private set; }
        public static void Initialize(Action libraryChanged)
        {
            Loader.Current = new Loader(libraryChanged);
            Loader.Current.Load(null);
        }

        private void Load(Action libraryChanged)
        {
            try
            {
                this.Id = Guid.NewGuid().ToString();
                string applicationLocation =
                    System.IO.Path.Combine(this._CacheRootLocation, this.Id);
                if (!Directory.Exists(applicationLocation))
                    Directory.CreateDirectory(applicationLocation);

                this.LoadExecutables(this._DomainRootLocation);

                libraryChanged?.Invoke();

                Basics.Console.Push(string.Empty, "Application is loaded successfully!", string.Empty, false);
            }
            catch (Exception)
            {
                Basics.Console.Push(string.Empty, "Application loading progress has been FAILED!", string.Empty, false);
            }

            // Do not block Application load
            ThreadPool.QueueUserWorkItem((state) => this.Cleanup());
        }

        private void LoadExecutables(string domainRootPath)
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

                        executableFI.CopyTo(applicationLocationFI.FullName, true);
                    }
                }

                DirectoryInfo domainChildrenDI =
                    new DirectoryInfo(System.IO.Path.Combine(domainDI.FullName, "Addons"));
                if (domainChildrenDI.Exists)
                    this.LoadExecutables(domainChildrenDI.FullName);
            }
        }

        private bool AvailableToDelete(DirectoryInfo applicationDI)
        {
            foreach (FileInfo fI in applicationDI.GetFiles())
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
            DirectoryInfo cacheRootDI =
                new DirectoryInfo(this._CacheRootLocation);
            if (!cacheRootDI.Exists)
                return;

            foreach (DirectoryInfo applicationDI in cacheRootDI.GetDirectories())
            {
                if (applicationDI.Name.Equals("PoolSessions") ||
                    applicationDI.Name.Equals(this.Id))
                    continue;

                if (!this.AvailableToDelete(applicationDI)) continue;
                
                try
                {
                    applicationDI.Delete(true);
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
