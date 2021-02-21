using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xeora.CLI.Extensions;

namespace Xeora.CLI.Basics
{
    public class Extract : ICommand
    {
        private string _XeoraDomainPath;
        private string _OutputLocation;
        // private bool _AutoApprove;
        private string _Password;
        private bool _ListContent;

        public Extract()
        {
            this._OutputLocation = 
                Path.GetFullPath($".{Path.DirectorySeparatorChar}");
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora extract OPTIONS XEORA_DOMAIN_PATH");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -p, --password PASSWORD     use a password for the encrypted xeora project domain");
            Console.WriteLine("   -o, --output PATH           output path to extract the xeora project domain");
            Console.WriteLine("   -l, --list                  list the files included in xeora domain");
            //Console.WriteLine("   -y                          auto approve questions");
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
                    case "-p":
                    case "--password":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("password has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        this._Password = args[aC + 1];

                        aC++;
                        break;
                    case "-o":
                    case "--output":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("output path has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._OutputLocation = args[aC + 1];
                        aC++;

                        break;
                    case "-l":
                    case "--list":
                        this._ListContent = true;

                        break;
//                    case "-y":
//                        this._AutoApprove = true;

//                        break;
                    default:
                        if (aC + 1 < args.Count)
                        {
                            this.PrintUsage();
                            Console.WriteLine("unrecognizable argument");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        this._XeoraDomainPath = Path.GetFullPath(args[aC]);

                        if (!Directory.Exists(this._XeoraDomainPath))
                        {
                            Console.WriteLine("xeora domain path is not exists");
                            return 1;
                        }

                        break;
                }
            }

            if (string.IsNullOrEmpty(this._XeoraDomainPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora domain path is required");
                Console.WriteLine();
                return 2;
            }

            if (!File.Exists(Path.Combine(this._XeoraDomainPath, "Content.xeora")))
            {
                this.PrintUsage();
                Console.WriteLine("xeora content file is not exists in the given path");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public async Task<int> Execute(IReadOnlyList<string> args)
        {
            int argumentsResult =
                this.SetArguments(args);
            if (argumentsResult != 0) return argumentsResult;
            
            try
            {
                byte[] passwordHash = null;

                if (!string.IsNullOrEmpty(this._Password))
                {
                    System.Security.Cryptography.MD5CryptoServiceProvider md5 = 
                        new System.Security.Cryptography.MD5CryptoServiceProvider();
                    passwordHash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(this._Password));
                }

                ExtractHelper extractHelper = 
                    new ExtractHelper();
                await Task.Factory.StartNew(
                    () => extractHelper.Extract(
                        Path.Combine(this._XeoraDomainPath, "Content.xeora"),
                        passwordHash,
                        this._ListContent,
                        this._OutputLocation
                    )
                );

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Extract operation has been failed!");
                Console.WriteLine($"   Reason: {e.Message}");
                return 1;
            }
        }
    }
}
