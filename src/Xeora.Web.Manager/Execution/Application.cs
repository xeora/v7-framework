using System;
using Xeora.Web.Basics;

namespace Xeora.Web.Manager.Execution
{
    public class Application
    {
        private readonly INegotiator _Negotiator;
        private readonly string _ExecutablesPath;
        private readonly string _ExecutableName;

        private ApplicationContext _Context;
        
        public Application(INegotiator negotiator, string executablesPath, string executableName)
        {
            this._Negotiator = negotiator;
            this._ExecutablesPath = executablesPath;
            this._ExecutableName = executableName;
        }

        public object Invoke(Basics.Context.Request.HttpMethod httpMethod, string[] classNames, string functionName, object[] functionParams, bool instanceExecute, ExecuterTypes executerType) =>
            this._Context.Invoke(httpMethod, classNames, functionName, functionParams, instanceExecute, executerType);

        public bool Load()
        {
            this._Context = 
                new ApplicationContext(this._Negotiator, this._ExecutablesPath, this._ExecutableName);
            this._Context.Load();

            return !this._Context.MissingFileException;
        }

        public void Unload()
        {
            if (this._Context == null) return;
            
            this._Context.Terminate();
            try
            {
                this._Context.Unload();
            }
            catch (Exception)
            {
                // TODO
                // We have "Cannot unload non-collectible AssemblyLoadContext." error
                // when we want to unload the AssemblyLoadContext with isCollectible: true
                // Because of this reason, we are just catching exceptions as long as we find a
                // suitable solution for this issue. 
            }
            this._Context = null;
        }
    }
}
