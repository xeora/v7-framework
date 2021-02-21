using System;
using System.Reflection;
using System.Threading.Tasks;
using Xeora.CLI.Basics;

namespace Xeora.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Program.PrintUsage();
                Environment.Exit(0);
            }

            ICommand cliCommand = null;

            switch (args[0])
            {
                case "framework":
                    cliCommand = new Framework();

                    break;
                case "create":
                    cliCommand = new Create();

                    break;
                case "run":
                    cliCommand = new Run();

                    break;
                case "compile":
                    cliCommand = new Compile();

                    break;
                case "publish":
                    cliCommand = new Publish();

                    break;
                case "extract":
                    cliCommand = new Extract();

                    break;
                case "-v":
                case "--version":
                    Program.PrintVersionText();
                    Environment.Exit(0);

                    break;
                default:
                    Program.PrintUsage();
                    Console.WriteLine("unrecognizable argument");
                    Console.WriteLine();
                    Environment.Exit(2);

                    break;
            }
            
            string[] filtered = 
                new string[args.Length - 1];
            Array.Copy(args, 1, filtered, 0, filtered.Length);
            
            Environment.Exit(await cliCommand.Execute(filtered));
        }
        
        private static void PrintVersionText()
        {
            System.Version vI = 
                Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"{vI?.Major}.{vI?.Minor}.{vI?.Build}");
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora COMMAND");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -v, --version               print CLI version information and quit");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("   compile                     compile the xeora project for release");
            Console.WriteLine("   create                      create the xeora project from template");
            Console.WriteLine("   framework                   download/update xeora framework executable");
            Console.WriteLine("   publish                     publish the xeora project to the target");
            Console.WriteLine("   run                         run the xeora project");
            Console.WriteLine();
        }
    }
}
