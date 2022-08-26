using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xeora.CLI.Extensions.Build;

namespace Xeora.CLI.Extensions
{
    public class CompileHelper
    {
        private int _LastPercent;

        public async Task Compile(string projectRoot, string[] domainPath, string password, string output, bool recursive, bool externalContent)
        {
            domainPath ??= Array.Empty<string>();
            
            if (domainPath.Length > 0)
            {
                DomainCompilerInfo domainCompilerInfo =
                    new DomainCompilerInfo(projectRoot, domainPath, password, output);

                await this.Compile(domainCompilerInfo, externalContent);

                if (!recursive) return;
            }

            IEnumerable<string> domains = 
                GetSubDomains(projectRoot, domainPath);
            foreach (string domain in domains)
            {
                string[] newDomainPath = 
                    new string[domainPath.Length + 1];
                Array.Copy(domainPath, 0, newDomainPath, 0, domainPath.Length);
                newDomainPath[^1] = domain;

                await this.Compile(projectRoot, newDomainPath, password, output, recursive, externalContent);
            }
        }

        private async Task Compile(DomainCompilerInfo domainCompilerInfo, bool externalContent)
        {
            this._LastPercent = -1;

            Console.WriteLine($"Compiling {string.Join('\\', domainCompilerInfo.DomainPath)}");

            Compiler xeoraCompiler = 
                new Compiler(domainCompilerInfo.DomainLocation);
            xeoraCompiler.Progress += this.UpdateProgress;

            domainCompilerInfo.RemoveTarget();
            this.AddFiles(domainCompilerInfo.DomainLocation, externalContent, ref xeoraCompiler);

            Stream contentFS = null;
            try
            {
                domainCompilerInfo.Setup();

                contentFS = 
                    new FileStream(domainCompilerInfo.OutputFile, FileMode.Create, FileAccess.ReadWrite);
                xeoraCompiler.CreateDomainFile(domainCompilerInfo.Password, ref contentFS);
            }
            finally
            {
                contentFS?.Dispose();
            }

            if (xeoraCompiler.PasswordHash != null)
            {
                Stream securedFS = null;
                try
                {
                    domainCompilerInfo.Setup();

                    securedFS = 
                        new FileStream(domainCompilerInfo.KeyFile, FileMode.Create, FileAccess.ReadWrite);
                    await securedFS.WriteAsync(xeoraCompiler.PasswordHash, 0, xeoraCompiler.PasswordHash.Length);
                }
                finally
                {
                    securedFS?.Dispose();
                }
            }

            Console.WriteLine();
        }

        private void AddFiles(string workingPath, bool externalContent, ref Compiler xeoraCompilerObj)
        {
            foreach (string path in Directory.GetDirectories(workingPath))
            {
                DirectoryInfo dI = new DirectoryInfo(path);

                if (string.Compare(dI.Name, "addons", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(dI.Name, "executables", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(dI.Name, "contents", StringComparison.OrdinalIgnoreCase) == 0 && externalContent)
                    continue;
                
                this.AddFiles(path, externalContent, ref xeoraCompilerObj);
            }

            foreach (string file in Directory.GetFiles(workingPath))
                xeoraCompilerObj.AddFile(file);
        }

        private void UpdateProgress(int current, int total)
        {
            int percent = 
                current * 100 / total;
            if (this._LastPercent == percent) return;
            
            this._LastPercent = percent;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("   ");
            Console.Write(percent.ToString().PadLeft(3));
            Console.Write("% ");
            for (int pC = 0; pC < this._LastPercent; pC++)
                Console.Write("*");
            if (this._LastPercent == 100)
                Console.WriteLine(" Completed");
        }

        private static IEnumerable<string> GetSubDomains(string projectRoot, string[] domainPath)
        {
            if (string.IsNullOrEmpty(projectRoot))
                return Array.Empty<string>();

            string domainLocation = Path.Combine(projectRoot, "Domains");

            domainPath ??= Array.Empty<string>();

            if (domainPath.Length > 0)
            {
                domainLocation = Path.Combine(domainLocation, domainPath[0], "Addons");
                for (int pC = 1; pC < domainPath.Length; pC++)
                    domainLocation = Path.Combine(domainLocation, domainPath[pC], "Addons");
            }

            DirectoryInfo domainRoot = 
                new DirectoryInfo(domainLocation);
            if (!domainRoot.Exists) return Array.Empty<string>();

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
                this.DomainLocation = MakePath(projectRoot, domainPath, false);
                this.Password = password;

                this.OutputPath = this.DomainLocation;
                if (!string.IsNullOrEmpty(outputPath))
                    this.OutputPath = MakePath(outputPath, domainPath, true);

                this.OutputFile = Path.Combine(this.OutputPath, "app.xeora");
                this.KeyFile = Path.Combine(this.OutputPath, "app.secure");
            }

            public string ProjectRoot { get; }
            public string[] DomainPath { get; }
            public string DomainLocation { get; }
            public string Password { get; }

            private string OutputPath { get; }
            public string OutputFile { get; }
            public string KeyFile { get; }

            private static string MakePath(string projectRoot, IReadOnlyList<string> domainPath, bool omitDomainsFolder)
            {
                string path = projectRoot;
                if (!omitDomainsFolder)
                    path = Path.Combine(path, "Domains");
                path = Path.Combine(path, domainPath[0]);

                for (int pC = 1; pC < domainPath.Count; pC++)
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
