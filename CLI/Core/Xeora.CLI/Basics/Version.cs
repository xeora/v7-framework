using System;
using System.IO;
using System.Reflection;

namespace Xeora.CLI.Basics
{
    public class Version : ICommand
    {
        private string _AssemblyLocation;

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora version OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -f FILE                     takes the release version of the specified xeora framework file (required)");
            Console.WriteLine();
        }

        public int SetArguments(string[] args)
        {
            for (int aC = 0; aC < args.Length; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-f":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora framework file should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._AssemblyLocation = args[aC + 1];
                        aC++;

                        break;
                }
            }

            if (string.IsNullOrEmpty(this._AssemblyLocation))
            {
                this.PrintUsage();
                Console.WriteLine("xeora framework file is required");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public int Execute()
        {
            try
            {
                string assemblyPath = Path.GetFullPath(this._AssemblyLocation);
                Assembly assembly = Assembly.LoadFile(assemblyPath);

                Console.Write(this.MakeVersionText(assembly));
                return 0;
            }
            catch (Exception)
            {
                Console.WriteLine("not possible to get the version");
                return 1;
            }
        }

        private bool CheckArgument(string[] argument, int index)
        {
            if (argument.Length <= index + 1)
                return false;

            string value = argument[index + 1];
            if (value.IndexOf("-") == 0)
                return false;

            return true;
        }

        private string MakeVersionText(Assembly assembly)
        {
            System.Version vI = assembly.GetName().Version;
            return string.Format("{0}.{1}.{2}", vI.Major, vI.Minor, vI.Build);
        }
    }
}
