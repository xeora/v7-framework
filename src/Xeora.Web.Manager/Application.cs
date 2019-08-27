using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Xeora.Web.Manager
{
    public class Application
    {
        private readonly List<string> _AssemblyPaths;
        private LibraryManager _LibraryManager;

        private readonly string _ExecutableName;

        private Application(string executableName)
        {
            this._ExecutableName = executableName;
            this._AssemblyPaths = new List<string>();
        }

        public void AddSearchPath(string assemblyPath)
        {
            if (!this._AssemblyPaths.Contains(assemblyPath))
                this._AssemblyPaths.Add(assemblyPath);
        }

        public object Invoke(Basics.Context.Request.HttpMethod httpMethod, string[] classNames, string functionName, object[] functionParams, bool instanceExecute, ExecuterTypes executerType) =>
            this._LibraryManager.Invoke(httpMethod, classNames, functionName, functionParams, instanceExecute, executerType);

        // Load must use the same appdomain because AppDomain logic is not supported in .NET Standard anymore
        private bool Load()
        {
            this._LibraryManager = 
                new LibraryManager(Loader.Current.Path, this._ExecutableName, this._AssemblyPaths.ToArray());
            this._LibraryManager.Load();

            return !this._LibraryManager.MissingFileException;
        }

        private void Unload() =>
            this._LibraryManager?.Dispose();
        
        private static readonly object PrepareLock = new object();
        private static readonly Dictionary<string, Application> ApplicationCache =
            new Dictionary<string, Application>();
        public static Application Prepare(string executableName)
        {
            string applicationKey =
                $"KEY-{Loader.Current.Path}_{executableName}";
            
            lock (Application.PrepareLock)
            {
                if (Application.ApplicationCache.ContainsKey(applicationKey))
                    return Application.ApplicationCache[applicationKey];

                Application application = 
                    new Application(executableName);
                application.AddSearchPath(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                application.AddSearchPath(Loader.Current.Path);

                if (!application.Load())
                    throw new FileLoadException();

                Application.ApplicationCache[applicationKey] = application;

                return application;
            }
        }

        public static void Dispose()
        {
            lock (Application.PrepareLock)
            {
                foreach (string key in Application.ApplicationCache.Keys)
                    Application.ApplicationCache[key].Unload();
            }
        }
    }
}
