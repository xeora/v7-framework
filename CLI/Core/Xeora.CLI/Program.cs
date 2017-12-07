using System;
using Xeora.CLI.Basics;

namespace Xeora.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommand cliCommand = null;

            switch (args[0])
            {
                case "compile":
                    cliCommand = new Compile();

                    break;
                case "create":
                    cliCommand = new Create();

                    break;
                case "framework":
                    cliCommand = new Framework();

                    break;
                case "publish":
                    cliCommand = new Publish();

                    break;
                case "version":
                    cliCommand = new Basics.Version();

                    break;
                default:
                    Program.PrintUsage();
                    Environment.Exit(0);

                    break;
            }

            if (args.Length == 1)
            {
                cliCommand.PrintUsage();
                Environment.Exit(0);
            }

            string[] filtered = new string[args.Length - 1];
            Array.Copy(args, 1, filtered, 0, filtered.Length);

            int result = cliCommand.SetArguments(filtered);
            if (result == -1)
                Environment.Exit(0);
            if (result > 0)
                Environment.Exit(result);

            Environment.Exit(
                cliCommand.Execute());
        }

        static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora COMMAND");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -v, --version               print CLI version information and quit");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("   compile                     compile the xeora project for release");
            Console.WriteLine("   create                      create the xeora project template");
            Console.WriteLine("   framework                   download/update xeora project framework");
            Console.WriteLine("   publish                     publish the xeora project to the target");
            Console.WriteLine("   version                     takes the release version of the specified xeora framework file");
            Console.WriteLine();
        }
    }
}
