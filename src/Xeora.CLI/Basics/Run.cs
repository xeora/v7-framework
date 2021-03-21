using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xeora.Web.Service;

namespace Xeora.CLI.Basics
{
    public class Run : ICommand
    {
        private string _SettingsFile;
        private string _Name;

        public Run()
        {
            this._SettingsFile = string.Empty;
            this._Name = string.Empty;
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora run OPTIONS [SETTINGS FILE]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -n, --name NAME             naming the execution");
            Console.WriteLine();
        }

        private int SetArguments(IReadOnlyList<string> args)
        {
            for (int aC = 0; aC < args.Count; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-n":
                    case "--name":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("name should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Name = args[aC + 1];
                        aC++;

                        break;
                    default:
                        if (aC + 1 < args.Count)
                        {
                            this.PrintUsage();
                            Console.WriteLine("unrecognizable argument");
                            Console.WriteLine();
                            return 2;
                        }

                        this._SettingsFile = Path.GetFullPath(args[aC]);
                        break;
                }
            }

            if (string.IsNullOrEmpty(this._SettingsFile))
                this._SettingsFile = "xeora.settings.json";

            return 0;
        }

        public async Task<int> Execute(IReadOnlyList<string> args)
        {
            int argumentsResult =
                this.SetArguments(args);
            if (argumentsResult != 0) return argumentsResult;
            
            try
            {
                Server server = 
                    new Server(this._SettingsFile, this._Name);
                return await server.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"XeoraEngine execution problem: {e.Message}");
                return 1;
            }
        }
    }
}
