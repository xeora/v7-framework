using System;
using System.IO;
using System.Threading;

namespace Xeora.Web.Manager
{
    public class Loader
    {
        private const string EXECUTABLES = "Executables";
        private const string ADDONS = "Addons";
        
        private readonly string _CacheRootLocation;
        private readonly string _DomainRootLocation;
        private readonly Watcher _Watcher;

        private Loader(Basics.Configuration.IXeora configuration, Action<string, string> reloadHandler)
        {
            this._CacheRootLocation =
                System.IO.Path.Combine(
                    configuration.Application.Main.TemporaryRoot,
                    configuration.Application.Main.WorkingPath.WorkingPathId
                );

            if (!Directory.Exists(this._CacheRootLocation))
                Directory.CreateDirectory(this._CacheRootLocation);

            this._DomainRootLocation =
                System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(
                        configuration.Application.Main.PhysicalRoot,
                        configuration.Application.Main.ApplicationRoot.FileSystemImplementation,
                        "Domains"
                    )
                );
            
            this._Watcher = 
                new Watcher(this._DomainRootLocation, () =>
                {
                    Basics.Console.Push(string.Empty, "Library changes have been detected!", string.Empty, false, true);
                    
                    // Reload
                    this.Load();
                    
                    // Notify
                    reloadHandler?.Invoke(this.Id, this.Path);
                });
        }

        public string Id { get; private set; }
        public string Path =>
            System.IO.Path.Combine(this._CacheRootLocation, this.Id);

        public static Loader Current { get; private set; }
        public static void Initialize(Basics.Configuration.IXeora configuration, Action<string, string> reloadHandler)
        {
            if (Loader.Current == null)
                Loader.Current = new Loader(configuration, reloadHandler);
            Loader.Current.Load();
        }
        
        private void Load()
        {
            try
            {
                this.Id = Guid.NewGuid().ToString();
                
                if (!Directory.Exists(this.Path))
                    Directory.CreateDirectory(this.Path);

                this.LoadExecutables(this._DomainRootLocation);
                this._Watcher.Start();
                
                Basics.Console.Push(string.Empty, "Application is prepared successfully!", string.Empty, false, true);
            }
            catch (Exception)
            {
                Basics.Console.Push(string.Empty, "Application preparation has been FAILED!", string.Empty, false, true, type: Basics.Console.Type.Error);

                return;
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
                    System.IO.Path.Combine(domain.FullName, Loader.EXECUTABLES);

                DirectoryInfo domainExecutables =
                    new DirectoryInfo(domainExecutablesLocation);
                if (domainExecutables.Exists)
                    this.CopyToTarget(domainExecutables, this.Path);

                DirectoryInfo domainChildren =
                    new DirectoryInfo(System.IO.Path.Combine(domain.FullName, Loader.ADDONS));
                if (domainChildren.Exists)
                    this.LoadExecutables(domainChildren.FullName);
            }
        }

        private void CopyToTarget(DirectoryInfo sourceRoot, string target)
        {
            foreach (FileInfo fI in sourceRoot.GetFiles())
            {
                FileInfo applicationLocation =
                    new FileInfo(
                        System.IO.Path.Combine(target, fI.Name));
                if (applicationLocation.Exists) continue;

                fI.CopyTo(applicationLocation.FullName, true);
            }

            foreach (DirectoryInfo dI in sourceRoot.GetDirectories())
            {
                DirectoryInfo applicationLocation =
                    new DirectoryInfo(
                        System.IO.Path.Combine(target, dI.Name));
                if (applicationLocation.Exists) continue;

                applicationLocation.Create();
                this.CopyToTarget(dI, applicationLocation.FullName);
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

            Basics.Console.Push(string.Empty, "Cache is cleaned up!", string.Empty, false, true);
        }
    }
}
