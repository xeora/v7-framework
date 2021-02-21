using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xeora.CLI.Basics
{
    public class Publish : ICommand
    {
        private readonly string[] XEORA_RELEASE_FOLDERS = { "Addons", "Executables" };
        private readonly string[] XEORA_FOLDERS = { "Addons", "Contents", "Executables", "Languages", "Templates" };

        private string _XeoraProjectPath;
        private string _OutputLocation;
        private string[] _Excludes;
        private bool _IncludeContent;
        private bool _AutoApprove;

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora publish OPTIONS XEORA_ROOT_PATH");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -o, --output PATH           output path of publishing xeora project (required)");
            Console.WriteLine("   -c, --content               includes external content located in to xeora project otherwise only domains");
            Console.WriteLine("   -e, --exclude DIRNAME       excludes comma separated folders from publishing");
            Console.WriteLine("   -y                          auto approve questions");
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
                    case "-c":
                    case "--content":
                        this._IncludeContent = true;

                        break;
                    case "-e":
                    case "--exclude":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("directory names have not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Excludes = args[aC + 1].Split(',');
                        aC++;

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

            if (string.IsNullOrEmpty(this._XeoraProjectPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora project path is required");
                Console.WriteLine();
                return 2;
            }

            if (string.IsNullOrEmpty(this._OutputLocation))
            {
                this.PrintUsage();
                Console.WriteLine("output path is required");
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
                DirectoryInfo xeoraProjectRoot =
                    new DirectoryInfo(this._XeoraProjectPath);
                Console.WriteLine($"Publishing Xeora Project: {xeoraProjectRoot.Name}");

                DirectoryInfo outputLocationRoot =
                    new DirectoryInfo(this._OutputLocation);
                if (outputLocationRoot.Exists)
                {
                    Console.Write("Do you approve to truncate the output location? (y/N) ");

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
                    
                    outputLocationRoot.Delete(true);
                }
                
                string domainRootPath = 
                    Path.Combine(this._XeoraProjectPath, "Domains");
                string outputRootPath =
                    Path.Combine(this._OutputLocation, "Domains");
                await this.PublishDomains(domainRootPath, outputRootPath);

                Console.WriteLine(" - xeora project publishing is completed!");

                if (this._IncludeContent)
                {
                    Console.WriteLine();
                    Console.WriteLine("Publishing External Contents of Xeora Project");

                    int totalCopied =
                        await this.PublishExternalContents();

                    Console.WriteLine(totalCopied > 0
                        ? " - external contents publishing is completed!"
                        : " - there is nothing to publish");
                }
                Console.WriteLine();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Publishing has been failed!");
                Console.WriteLine($"   Reason: {e.Message}");
                return 1;
            }
        }

        private async Task PublishDomains(string domainRootPath, string outputRootPath)
        {
            DirectoryInfo domainRootDI = 
                new DirectoryInfo(domainRootPath);
            DirectoryInfo outputRootDI = 
                new DirectoryInfo(outputRootPath);
            if (!outputRootDI.Exists) outputRootDI.Create();
            
            foreach (DirectoryInfo domainDI in domainRootDI.GetDirectories())
            {
                DirectoryInfo outputContentDI = 
                    new DirectoryInfo(Path.Combine(outputRootDI.FullName, domainDI.Name));
                if (!outputContentDI.Exists) outputContentDI.Create();

                // Check for Release Version
                string outputFile = 
                    Path.Combine(domainDI.FullName, "Content.xeora");
                string keyFile = 
                    Path.Combine(domainDI.FullName, "Content.secure");

                string[] xeoraFolders = this.XEORA_FOLDERS; // Consider that it is not a compiled xeora project
                if (File.Exists(outputFile))
                {
                    WriteUpdateToConsole("moving", outputFile, outputContentDI.FullName);
                    File.Move(outputFile, Path.Combine(outputContentDI.FullName, "Content.xeora"));
                    WriteUpdateToConsole("done!", string.Empty, string.Empty);
                    if (File.Exists(keyFile))
                    {
                        WriteUpdateToConsole("moving", keyFile, outputContentDI.FullName);
                        File.Move(keyFile, Path.Combine(outputContentDI.FullName, "Content.secure"));
                        WriteUpdateToConsole("done!", string.Empty, string.Empty);
                    }
                    
                    xeoraFolders = this.XEORA_RELEASE_FOLDERS; // We know that it is a compiled xeora project
                }

                foreach (DirectoryInfo domainContentDI in domainDI.GetDirectories())
                {
                    if (Array.IndexOf(xeoraFolders, domainContentDI.Name) == -1) continue;
                    
                    DirectoryInfo targetContentDI =
                        new DirectoryInfo(Path.Combine(outputContentDI.FullName, domainContentDI.Name));
                    if (!targetContentDI.Exists) targetContentDI.Create();

                    if (string.Compare(domainContentDI.Name, "Addons", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        await this.Copy(domainContentDI, targetContentDI);
                        continue;
                    }

                    await this.PublishDomains(domainContentDI.FullName, targetContentDI.FullName);
                }
            }
        }

        private async Task<int> PublishExternalContents()
        {
            DirectoryInfo xeoraProjectDI = 
                new DirectoryInfo(this._XeoraProjectPath);

            int totalCopied = 0;
            foreach(DirectoryInfo item in xeoraProjectDI.GetDirectories())
            {
                if (string.Compare(item.Name, "Domains", StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                if (this._Excludes != null && Array.IndexOf(this._Excludes, item.Name) > -1)
                    continue;

                DirectoryInfo targetItem =
                    new DirectoryInfo(Path.Combine(this._OutputLocation, item.Name));
                if (!targetItem.Exists) targetItem.Create();
                
                await this.Copy(item, targetItem);
                totalCopied++;
            }
            
            foreach(FileInfo item in xeoraProjectDI.GetFiles())
            {
                if (this._Excludes != null && Array.IndexOf(this._Excludes, item.Name) > -1)
                    continue;

                string targetItemFullName =
                    Path.Combine(this._OutputLocation, item.Name);
                
                WriteUpdateToConsole("copying", item.FullName, targetItemFullName);
                item.CopyTo(targetItemFullName);
                WriteUpdateToConsole("done!", string.Empty, string.Empty);
                
                totalCopied++;
            }

            return totalCopied;
        }

        private async Task Copy(DirectoryInfo source, DirectoryInfo target)
        {
            foreach(DirectoryInfo item in source.GetDirectories())
            {
                DirectoryInfo targetItem = 
                    new DirectoryInfo(Path.Combine(target.FullName, item.Name));
                if (!targetItem.Exists) targetItem.Create();

                await this.Copy(item, targetItem);
            }

            foreach(FileInfo item in source.GetFiles())
            {
                FileInfo targetItem =
                    new FileInfo(Path.Combine(target.FullName, item.Name));
                if (targetItem.Exists) targetItem.Delete();

                WriteUpdateToConsole("copying", item.FullName, targetItem.FullName);
                item.CopyTo(targetItem.FullName);
                WriteUpdateToConsole("done!", string.Empty, string.Empty);
            }
        }

        private static void WriteUpdateToConsole(string action, string sourcePath, string targetPath)
        {
            if (!string.IsNullOrEmpty(sourcePath) &&
                !string.IsNullOrEmpty(targetPath))
            {
                Console.Write("   ");
                Console.Write(action);
                Console.Write(" ");
                if (sourcePath.Length > 50)
                    sourcePath = sourcePath.Substring(sourcePath.Length - 50);
                Console.Write(sourcePath.PadLeft(50));
                Console.Write(" -> ");
                if (targetPath.Length > 50)
                    targetPath = targetPath.Substring(targetPath.Length - 50);
                Console.Write(targetPath.PadRight(50));

                return;
            } 
                
            Console.Write(" ");
            Console.Write(action);
            Console.WriteLine();
        }
    }
}
