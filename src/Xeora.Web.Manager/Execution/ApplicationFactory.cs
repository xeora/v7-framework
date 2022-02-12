﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xeora.Web.Basics;

namespace Xeora.Web.Manager.Execution
{
    public class ApplicationFactory
    {
        public static readonly object InstanceCreationLock = new();
        private static readonly object GetOrCreateLock = new();
        
        private readonly INegotiator _Negotiator;
        private readonly string _ExecutablesPath;
        private readonly Dictionary<string, Application> _Cache;

        private ApplicationFactory(INegotiator negotiator, string executablesPath)
        {
            this._Negotiator = negotiator;
            this._ExecutablesPath = executablesPath;
            this._Cache = new Dictionary<string, Application>();
        }

        private static readonly object InitLock = new object();
        private static ApplicationFactory _current;
        public static void Initialize(INegotiator negotiator, string executablesPath)
        {
            Monitor.Enter(ApplicationFactory.InitLock);
            try
            {
                if (ApplicationFactory._current != null)
                    ApplicationFactory._current.Unload();
                
                ApplicationFactory._current = 
                    new ApplicationFactory(negotiator, executablesPath);
            }
            finally
            {
                Monitor.Exit(ApplicationFactory.InitLock);
            }
        }

        public static void Terminate() => 
            ApplicationFactory._current?.Unload();
        
        public static Application Prepare(string executableName) =>
            ApplicationFactory._current?.GetOrCreate(executableName);

        private Application GetOrCreate(string executableName)
        {
            Monitor.Enter(ApplicationFactory.GetOrCreateLock);
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
                Monitor.Exit(ApplicationFactory.GetOrCreateLock);
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
