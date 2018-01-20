using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Xeora.Web.Manager
{
    public class Application
    {
        private List<string> _AssemblyPaths;
        private LibraryExecuter _LibraryExecuter;

        private string _ExecutableName;

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

        public object Invoke(string httpMethodType, string[] classNames, string functionName, object[] functionParams, bool instanceExecute, string executerType) =>
            this._LibraryExecuter.Invoke(httpMethodType, classNames, functionName, functionParams, instanceExecute, executerType);

        // Load must use the same appdomain because AppDomain logic is not supported in .NET Standard anymore
        private bool Load()
        {
            this._LibraryExecuter = new LibraryExecuter(Loader.Current.Path, this._ExecutableName, this._AssemblyPaths.ToArray());
            return !this._LibraryExecuter.MissingFileException;
        }

        private static ConcurrentDictionary<string, Application> _ApplicationCache =
            new ConcurrentDictionary<string, Application>();
        public static Application Prepare(string executableName)
        {
            Loader.Initialize();

            string applicationKey =
                string.Format("KEY-{0}_{1}", Loader.Current.Path, executableName);

            Application application;
            if (!Application._ApplicationCache.TryGetValue(applicationKey, out application))
            {
                application = new Application(executableName);
                application.AddSearchPath(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                application.AddSearchPath(Loader.Current.Path);

                if (!application.Load())
                {
                    Loader.Reload();
                    return Application.Prepare(executableName);
                }

                if (!Application._ApplicationCache.TryAdd(applicationKey, application))
                    return Application.Prepare(executableName);
            }

            return application;
        }

        public static void Dispose()
        {
            foreach(string key in Application._ApplicationCache.Keys)
            {
                Application application;
                if (Application._ApplicationCache.TryRemove(key, out application))
                    application._LibraryExecuter.Dispose();
            }
        }
    }
}
