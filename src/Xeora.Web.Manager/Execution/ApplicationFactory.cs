using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xeora.Web.Basics;

namespace Xeora.Web.Manager.Execution
{
    public class ApplicationFactory
    {
        private readonly INegotiator _Negotiator;
        private readonly string _ExecutablesPath;
        private readonly object _GetOrCreateLock;
        private readonly Dictionary<string, Application> _Cache;

        private ApplicationFactory(INegotiator negotiator, string executablesPath)
        {
            this._Negotiator = negotiator;
            this._ExecutablesPath = executablesPath;
            this._GetOrCreateLock = new object();
            this._Cache = new Dictionary<string, Application>();
        }

        private static readonly object InitLock = new object();
        private static ApplicationFactory _Current;
        public static void Initialize(INegotiator negotiator, string executablesPath)
        {
            Monitor.Enter(ApplicationFactory.InitLock);
            try
            {
                if (ApplicationFactory._Current != null)
                    ApplicationFactory._Current.Unload();
                
                ApplicationFactory._Current = 
                    new ApplicationFactory(negotiator, executablesPath);
            }
            finally
            {
                Monitor.Exit(ApplicationFactory.InitLock);
            }
        }

        public static void Terminate() => 
            ApplicationFactory._Current?.Unload();
        
        public static Application Prepare(string executableName) =>
            ApplicationFactory._Current?.GetOrCreate(executableName);

        private Application GetOrCreate(string executableName)
        {
            Monitor.Enter(this._GetOrCreateLock);
            try
            {
                string applicationKey =
                    $"KEY-{this._ExecutablesPath}_{executableName}";
                
                if (this._Cache.ContainsKey(applicationKey))
                    return this._Cache[applicationKey];

                Application application =
                    new Application(this._Negotiator, this._ExecutablesPath, executableName);

                if (!application.Load())
                    throw new FileLoadException();

                this._Cache[applicationKey] = application;

                return application;
            }
            finally
            {
                Monitor.Exit(this._GetOrCreateLock);
            }
        }

        private void Unload()
        {
            foreach (string key in this._Cache.Keys)
                this._Cache[key].Unload();
            
            if (this._Cache.Count > 0)
                Console.Push(string.Empty, "Applications are unloaded!", string.Empty, false, true);
        }
    }
}
