using System;
using System.IO;

namespace Xeora.CLI.Basics
{
    public class Publish : ICommand
    {
        private string[] XEORARELEASEFOLDERS = { "Addons", "Executables" };
        private string[] XEORAFOLDERS = { "Addons", "Contents", "Executables", "Languages", "Templates" };

        private string _XeoraProjectPath;
        private string _OutputLocation;
        private string[] _Excludes;
        private bool _IncludeContent;

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora publish OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -x, --xeora PATH            xeora project root path (required)");
            Console.WriteLine("   -o, --output PATH           output path of publish xeora project (required)");
            Console.WriteLine("   -c, --content               includes xeora project contents other than only domains");
            Console.WriteLine("   -e, --exclude DIRNAME       excludes comma separated folders from publishing");
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
                    case "-c":
                    case "--content":
                        this._IncludeContent = true;

                        break;
                    case "-e":
                    case "--exclude":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("directory names have not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Excludes = args[aC + 1].Split(',');
                        aC++;

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

            if (string.IsNullOrEmpty(this._OutputLocation))
            {
                this.PrintUsage();
                Console.WriteLine("output path is required");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public int Execute()
        {
            try
            {
                DirectoryInfo xeoraProjectRoot =
                    new DirectoryInfo(this._XeoraProjectPath);
                Console.WriteLine(string.Format("Publishing Xeora Project: {0}", xeoraProjectRoot.Name));

                string domainRootPath = 
                    Path.Combine(this._XeoraProjectPath, "Domains");
                string outputRootPath =
                    Path.Combine(this._OutputLocation, "Domains");
                this.PublishDomains(domainRootPath, outputRootPath);

                this.WriteUpdateToConsole("Completed", string.Empty, string.Empty);

                if (this._IncludeContent)
                {
                    Console.WriteLine();
                    Console.WriteLine("Publishing External Contents of Xeora Project");

                    this.PublishExternalContents();

                    this.WriteUpdateToConsole("Completed", string.Empty, string.Empty);
                }
                Console.WriteLine();

                return 0;
            }
            catch
            {
                Console.WriteLine("publishing has been FAILED!");
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

        private void PublishDomains(string domainRootPath, string outputRootPath)
        {
            DirectoryInfo domainRootDI = 
                new DirectoryInfo(domainRootPath);
            DirectoryInfo outputRootDI = 
                new DirectoryInfo(outputRootPath);
            if (!outputRootDI.Exists)
                outputRootDI.Create();
            
            foreach (DirectoryInfo domainDI in domainRootDI.GetDirectories())
            {
                DirectoryInfo outputContentDI = 
                    new DirectoryInfo(Path.Combine(outputRootDI.FullName, domainDI.Name));
                if (!outputContentDI.Exists)
                    outputContentDI.Create();

                // Check for Release Version
                string outputFile = Path.Combine(domainDI.FullName, "Content.xeora");
                string keyFile = Path.Combine(domainDI.FullName, "Content.secure");

                string[] xeoraFolders = this.XEORAFOLDERS;
                if (File.Exists(outputFile))
                {
                    this.WriteUpdateToConsole("moving", outputFile, outputContentDI.FullName);
                    File.Move(outputFile, Path.Combine(outputContentDI.FullName, "Content.xeora"));
                    if (File.Exists(keyFile))
                    {
                        this.WriteUpdateToConsole("moving", keyFile, outputContentDI.FullName);
                        File.Move(keyFile, Path.Combine(outputContentDI.FullName, "Content.secure"));
                    }
                    
                    xeoraFolders = this.XEORARELEASEFOLDERS;
                }

                foreach (DirectoryInfo domainContentDI in domainDI.GetDirectories())
                {
                    if (Array.IndexOf<string>(xeoraFolders, domainContentDI.Name) > -1)
                    {
                        DirectoryInfo targetContentDI =
                            new DirectoryInfo(Path.Combine(outputContentDI.FullName, domainContentDI.Name));
                        if (!targetContentDI.Exists)
                            targetContentDI.Create();

                        if (string.Compare(domainContentDI.Name, "Addons", true) != 0)
                        {
                            this.Copy(domainContentDI, targetContentDI);
                            continue;
                        }

                        this.PublishDomains(domainContentDI.FullName, targetContentDI.FullName);
                    }
                }
            }
        }

        private void PublishExternalContents()
        {
            DirectoryInfo xeoraProjectDI = 
                new DirectoryInfo(this._XeoraProjectPath);

            foreach(DirectoryInfo item in xeoraProjectDI.GetDirectories())
            {
                if (string.Compare(item.Name, "Domains", true) == 0)
                    continue;

                if (this._Excludes != null && Array.IndexOf<string>(this._Excludes, item.Name) > -1)
                    continue;

                DirectoryInfo targetItem =
                    new DirectoryInfo(Path.Combine(this._OutputLocation, item.Name));
                if (!targetItem.Exists)
                    targetItem.Create();
                
                this.Copy(item, targetItem);
            }
        }

        private void Copy(DirectoryInfo source, DirectoryInfo target)
        {
            foreach(DirectoryInfo item in source.GetDirectories())
            {
                DirectoryInfo targetItem = 
                    new DirectoryInfo(Path.Combine(target.FullName, item.Name));
                if (!targetItem.Exists)
                    targetItem.Create();

                this.WriteUpdateToConsole("copying", item.FullName, targetItem.FullName);
                this.Copy(item, targetItem);
            }

            foreach(FileInfo item in source.GetFiles())
            {
                FileInfo targetItem =
                    new FileInfo(Path.Combine(target.FullName, item.Name));
                if (targetItem.Exists)
                    targetItem.Delete();

                this.WriteUpdateToConsole("copying", item.FullName, targetItem.FullName);
                item.CopyTo(targetItem.FullName);
            }
        }

        private void WriteUpdateToConsole(string action, string sourcePath, string targetPath)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write('\t');
            Console.Write(action.PadLeft(10));
            if (!string.IsNullOrEmpty(sourcePath) &&
                !string.IsNullOrEmpty(targetPath))
            {
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
                
            Console.Write("".PadRight(105));
            Console.WriteLine();
        }
    }
}
