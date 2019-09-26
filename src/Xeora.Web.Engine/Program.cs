using System;
using Xeora.Web.Service;

namespace Xeora.Web.Engine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string configurationFilePath = string.Empty;
            if (args != null && args.Length > 0)
                configurationFilePath = args[0];
            
            WebServer webServer = new WebServer(configurationFilePath);
			int exitCode = webServer.Start();

            Environment.Exit(exitCode);
        }
    }
}
