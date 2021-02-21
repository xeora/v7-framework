using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xeora.CLI.Extensions;

namespace Xeora.CLI.Basics
{
    public class Compile : ICommand
    {
        private class DomainInfo
        {
            public string DomainIdRoute { get; set; }
            public string Password { get; set; }

            public string[] DomainPath =>
                this.DomainIdRoute.Split("\\");
            public bool HasDomainIdRoute =>
                !string.IsNullOrEmpty(this.DomainIdRoute);
            public bool PasswordEnabled =>
                !string.IsNullOrEmpty(this.Password);
        }
        
        private string _XeoraProjectPath;
        private readonly List<DomainInfo> _DomainInfos;
        private string _OutputLocation;
        private bool _Recursive;
        private bool _AutoApprove;
        private string _GenericPassword;
        private bool _Publish;

        public Compile()
        {
            this._DomainInfos = new List<DomainInfo>();
            this._Recursive = false;
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora compile OPTIONS XEORA_ROOT_PATH");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -d, --domain DOMAINIDROUTE  domainid route of xeora project to compile. Ex: DOMAINID\\ADDONID");
            Console.WriteLine("   -p, --password PASSWORD     assign a password to encrypt the xeora project domain");
            Console.WriteLine("   -o, --output PATH           output path of compiled xeora project domain (required when used publish argument)");
            Console.WriteLine("   -r, --recursive             compile sub domains of domain recursively");
            Console.WriteLine("   -u, --publish               publish the result to the output path");
            Console.WriteLine("   -y                          auto approve questions");
            Console.WriteLine();
        }

        private int SetArguments(IReadOnlyList<string> args)
        {
            int domainCount = 0;
            int passwordCount = 0;
            
            for (int aC = 0; aC < args.Count; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-d":
                    case "--domain":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("domainid has not been specified");
                            Console.WriteLine();
                            return 2;
                        }

                        if (domainCount < this._DomainInfos.Count)
                            this._DomainInfos[domainCount].DomainIdRoute = args[aC + 1];
                        else
                        {
                            this._DomainInfos.Add(new DomainInfo {DomainIdRoute = args[aC + 1]});
                            domainCount++;
                        }
                        
                        aC++;
                        break;
                    case "-p":
                    case "--password":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("password has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        if (passwordCount < this._DomainInfos.Count)
                            this._DomainInfos[^1].Password = args[aC + 1];
                        else
                        {
                            this._DomainInfos.Add(new DomainInfo {Password = args[aC + 1]});
                            passwordCount++;
                        }

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
                    case "-r":
                    case "--recursive":
                        this._Recursive = true;

                        break;
                    case "-u":
                    case "--publish":
                        this._Publish = true;

                        break;
                    case "-y":
                        this._AutoApprove = true;

                        break;
                    default:
                        if (aC + 1 < args.Count)
                        {
                            this.PrintUsage();
                            Console.WriteLine("unrecognizable argument");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        this._XeoraProjectPath = Path.GetFullPath(args[aC]);

                        if (!Directory.Exists(this._XeoraProjectPath))
                        {
                            Console.WriteLine("xeora project path is not exists");
                            return 1;
                        }

                        break;
                }
            }

            if (this._Publish && string.IsNullOrEmpty(this._OutputLocation))
            {
                this.PrintUsage();
                Console.WriteLine("output location is required when publish argument is used");
                Console.WriteLine();
                return 2;
            }
            
            if (string.IsNullOrEmpty(this._XeoraProjectPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora project path is required");
                Console.WriteLine();
                return 2;
            }

            if (domainCount == passwordCount) return 0;
        
            for (int i = 0; i < this._DomainInfos.Count; i++)
            {
                DomainInfo domainInfo =
                    this._DomainInfos[i];
                        
                if (domainInfo.HasDomainIdRoute) continue;
                
                if (!domainInfo.PasswordEnabled)
                {
                    this.PrintUsage();
                    Console.WriteLine("faulty definition of domainidroutes and passwords");
                    Console.WriteLine();
                    return 2;
                }

                if (!string.IsNullOrEmpty(this._GenericPassword))
                {
                    this.PrintUsage();
                    Console.WriteLine("detected more than one generic password assignment");
                    Console.WriteLine();
                    return 2;
                }
                    
                this._GenericPassword = domainInfo.Password;
                
                this._DomainInfos.RemoveAt(i);
                i--;
            }

            if (this._DomainInfos.Count == 0) return 0;
            
            Console.WriteLine("DomainIdRoute and Password quantity does not match. Please");
            Console.WriteLine("approve the following list;");
            Console.WriteLine();

            foreach (DomainInfo domainInfo in this._DomainInfos)
                Console.WriteLine($"{domainInfo.DomainIdRoute}: {(domainInfo.PasswordEnabled || !string.IsNullOrEmpty(this._GenericPassword) ? "ENCRYPTED" : "PLAIN")}");

            Console.WriteLine();
            Console.Write("Do you approve the list? (y/N) ");

            if (this._AutoApprove)
                Console.WriteLine("y");
            else
            {
                string answer =
                    Console.ReadLine()?.ToLowerInvariant();
                if (string.IsNullOrEmpty(answer) ||
                    string.CompareOrdinal(answer, "y") != 0 &&
                    string.CompareOrdinal(answer, "yes") != 0)
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
                if (this._DomainInfos.Count == 0)
                    this._DomainInfos.AddRange(this.FetchDomains());
                
                foreach (DomainInfo dI in this._DomainInfos)
                {
                    CompileHelper compileHelper = 
                        new CompileHelper();
                    await compileHelper.Compile(
                        this._XeoraProjectPath,
                        dI.DomainPath,
                        dI.Password,
                        !this._Publish ? this._OutputLocation : string.Empty,
                        this._Recursive
                    );
                }

                if (!this._Publish) return 0;

                List<string> publishArgs = 
                    new List<string>();
                publishArgs.AddRange(new[] {"-o", this._OutputLocation, "-c"});
                
                if (this._AutoApprove)
                    publishArgs.Add("-y");
                
                publishArgs.Add(this._XeoraProjectPath);
                
                Publish publish = 
                    new Publish();
                await publish.Execute(publishArgs);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Compile operation has been failed!");
                Console.WriteLine($"   Reason: {e.Message}");
                return 1;
            }
        }

        private IEnumerable<DomainInfo> FetchDomains()
        {
            DirectoryInfo domainsPath =
                new DirectoryInfo(Path.Combine(this._XeoraProjectPath, "Domains"));

            List<DomainInfo> domainInfos =
                new List<DomainInfo>();
            foreach (DirectoryInfo dI in domainsPath.GetDirectories())
            {
                domainInfos.Add(new DomainInfo
                {
                    DomainIdRoute = dI.Name, 
                    Password = !string.IsNullOrEmpty(this._GenericPassword) ? this._GenericPassword : string.Empty
                });
            }

            return domainInfos.ToArray();
        }
    }
}
