using System;
using System.Collections.Generic;
using System.IO;
using Xeora.Extension.Tools;

namespace Xeora.CLI.Tools
{
    public class CompileHelper
    {
        private int _LastPercent;

        public void Compile(string projectRoot, string[] domainPath, string password, string output, bool recursive)
        {
            if (domainPath == null)
                domainPath = new string[] { };
            
            if (domainPath.Length > 0)
            {
                DomainCompilerInfo domainCompilerInfo =
                    new DomainCompilerInfo(projectRoot, domainPath, password, output);

                this.Compile(domainCompilerInfo);

                if (!recursive)
                    return;
            }

            string[] domains = this.GetSubDomains(projectRoot, domainPath);
            foreach (string domain in domains)
            {
                string[] newDomainPath = new string[domainPath.Length + 1];
                Array.Copy(domainPath, 0, newDomainPath, 0, domainPath.Length);
                newDomainPath[newDomainPath.Length - 1] = domain;

                this.Compile(projectRoot, newDomainPath, password, output, recursive);
            }
        }

        private void Compile(DomainCompilerInfo domainCompilerInfo)
        {
            this._LastPercent = -1;

            Console.WriteLine(string.Format("Compiling {0}", string.Join('\\', domainCompilerInfo.DomainPath)));

            Compiler xeoraCompiler = new Compiler(domainCompilerInfo.DomainLocation);
            xeoraCompiler.Progress += new Compiler.ProgressEventHandler(this.UpdateProgress);

            domainCompilerInfo.RemoveTarget();
            this.AddFiles(domainCompilerInfo.DomainLocation, ref xeoraCompiler);

            Stream contentFS = null;
            try
            {
                domainCompilerInfo.Setup();

                contentFS = new FileStream(domainCompilerInfo.OutputFile, FileMode.Create, FileAccess.ReadWrite);
                xeoraCompiler.CreateDomainFile(domainCompilerInfo.Password, ref contentFS);
            }
            finally
            {
                if (contentFS != null)
                    contentFS.Close();

                GC.SuppressFinalize(contentFS);
            }

            if (xeoraCompiler.PasswordHash != null)
            {
                Stream securedFS = null;
                try
                {
                    domainCompilerInfo.Setup();

                    securedFS = new FileStream(domainCompilerInfo.KeyFile, FileMode.Create, FileAccess.ReadWrite);
                    securedFS.Write(xeoraCompiler.PasswordHash, 0, xeoraCompiler.PasswordHash.Length);
                }
                finally
                {
                    if (securedFS != null)
                        securedFS.Close();

                    GC.SuppressFinalize(securedFS);
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        private void AddFiles(string workingPath, ref Compiler xeoraCompilerObj)
        {
            foreach (string path in Directory.GetDirectories(workingPath))
            {
                DirectoryInfo DI = new DirectoryInfo(path);

                if (string.Compare(DI.Name, "addons", true) != 0 &&
                    string.Compare(DI.Name, "executables", true) != 0)
                    this.AddFiles(path, ref xeoraCompilerObj);
            }

            foreach (string file in Directory.GetFiles(workingPath))
                xeoraCompilerObj.AddFile(file);
        }

        private void UpdateProgress(int current, int total)
        {
            int percent = (current * 100) / total;

            if (this._LastPercent != percent)
            {
                this._LastPercent = percent;

                int curPos = Console.CursorLeft;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write('\t');
                Console.Write(percent.ToString().PadLeft(3));
                Console.Write("% ");
                for (int pC = 0; pC < this._LastPercent; pC++)
                    Console.Write("*");
                if (this._LastPercent == 100)
                    Console.Write(" Completed");
            }
        }

        private string[] GetSubDomains(string projectRoot, string[] domainPath)
        {
            if (string.IsNullOrEmpty(projectRoot))
                return new string[] { };

            string domainLocation = Path.Combine(projectRoot, "Domains");

            if (domainPath == null)
                domainPath = new string[] { };

            if (domainPath.Length > 0)
            {
                domainLocation = Path.Combine(domainLocation, domainPath[0], "Addons");
                for (int pC = 1; pC < domainPath.Length; pC++)
                    domainLocation = Path.Combine(domainLocation, domainPath[pC], "Addons");
            }

            DirectoryInfo domainRoot = new DirectoryInfo(domainLocation);

            if (!domainRoot.Exists)
                return new string[] { };

            List<string> domains = new List<string>();

            foreach (DirectoryInfo domain in domainRoot.GetDirectories())
                domains.Add(domain.Name);

            return domains.ToArray();
        }

        private class DomainCompilerInfo
        {
            public DomainCompilerInfo(string projectRoot, string[] domainPath, string password, string outputPath)
            {
                this.ProjectRoot = projectRoot;
                this.DomainPath = domainPath;
                this.DomainLocation = this.MakePath(projectRoot, domainPath, false);
                this.Password = password;

                this.OutputPath = this.DomainLocation;
                if (!string.IsNullOrEmpty(outputPath))
                    this.OutputPath = this.MakePath(outputPath, domainPath, true);

                this.OutputFile = Path.Combine(this.OutputPath, "Content.xeora");
                this.KeyFile = Path.Combine(this.OutputPath, "Content.secure");
            }

            public string ProjectRoot { get; private set; }
            public string[] DomainPath { get; private set; }
            public string DomainLocation { get; private set; }
            public string Password { get; private set; }

            private string OutputPath { get; set; }
            public string OutputFile { get; private set; }
            public string KeyFile { get; private set; }

            private string MakePath(string projectRoot, string[] domainPath, bool omitDomainsFolder)
            {
                string path = projectRoot;
                if (!omitDomainsFolder)
                    path = Path.Combine(path, "Domains");
                path = Path.Combine(path, domainPath[0]);

                for (int pC = 1; pC < domainPath.Length; pC++)
                    path = Path.Combine(path, "Addons", domainPath[pC]);

                return path;
            }

            public void Setup()
            {
                if (!Directory.Exists(this.OutputPath))
                    Directory.CreateDirectory(this.OutputPath);
            }

            public void RemoveTarget()
            {
                try
                {
                    File.Delete(this.OutputFile);
                }
                catch (Exception)
                { /* Just Handle Exceptions*/ }

                try
                {
                    File.Delete(this.KeyFile);
                }
                catch (Exception)
                { /* Just Handle Exceptions*/ }
            }
        }
    }
}
