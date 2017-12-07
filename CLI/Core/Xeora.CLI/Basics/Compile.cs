using System;
using System.Collections.Generic;
using System.IO;
using Xeora.CLI.Tools;

namespace Xeora.CLI.Basics
{
    public class Compile : ICommand
    {
        private string _XeoraProjectPath;
        private List<string> _DomainIDPaths;
        private List<string> _Passwords;
        private string _OutputLocation;
        private bool _Recursive;

        public Compile()
        {
            this._DomainIDPaths = new List<string>();
            this._Passwords = new List<string>();
            this._Recursive = false;
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora compile OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -x, --xeora PATH            xeora project root path (required)");
            Console.WriteLine("   -d, --domain DOMAINIDPATH   domainid path of xeora project to compile");
            Console.WriteLine("   -p, --password PASSWORD     assign a password to encrypt the xeora project domain");
            Console.WriteLine("   -o, --output PATH           output path of compiled xeora project domain");
            Console.WriteLine("   -r, --recursive             compile sub domains of domain recursively");
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
                    case "-x":
                    case "--xeora":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora project root path is required");
                            Console.WriteLine();
                            return 2;
                        }
                        this._XeoraProjectPath = args[aC + 1];
                        aC++;

                        if (!Directory.Exists(this._XeoraProjectPath))
                        {
                            Console.WriteLine("xeora project root path is not exists");
                            return 1;
                        }

                        break;
                    case "-d":
                    case "--domain":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("domainid has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._DomainIDPaths.AddRange(args[aC + 1].Split(','));
                        aC++;

                        break;
                    case "-p":
                    case "--password":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("password has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Passwords.AddRange(args[aC + 1].Split(','));
                        aC++;

                        break;
                    case "-o":
                    case "--output":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("output path has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._OutputLocation = args[aC + 1];
                        aC++;

                        break;
                    case "-r":
                    case "--recursive":
                        this._Recursive = true;

                        break;
                }
            }

            if (string.IsNullOrEmpty(this._XeoraProjectPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora project root path is required");
                Console.WriteLine();
                return 2;
            }

            if (this._Passwords.Count > 0 && this._DomainIDPaths.Count != this._Passwords.Count)
            {
                this.PrintUsage();
                Console.WriteLine("domains and passwords should match");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public int Execute()
        {
            try
            {
                for (int dC = 0; dC < this._DomainIDPaths.Count; dC++)
                {
                    CompileHelper compileHelper = new CompileHelper();
                    compileHelper.Compile(this._XeoraProjectPath, this._DomainIDPaths[dC].Split('\\'), this._Passwords[dC], this._OutputLocation, this._Recursive);
                }

                return 0;
            }
            catch
            {
                Console.WriteLine("compilation has been FAILED!");
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
    }
}
