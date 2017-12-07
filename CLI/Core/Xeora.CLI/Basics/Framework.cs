using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xeora.CLI.Basics
{
    public class Framework : ICommand
    {
        private const string XEORAORG = "http://www.xeora.org/Releases/v7";

        private string[] _AvailArchitectures = { "x86", "x64" };
        private string[] _AvailPlatforms = { "core", "standard" };

        private string _XeoraFrameworkPath;
        private string _Architecture;
        private string _Platform;
        private string _Release;

        private long _LastPercent;

        public Framework()
        {
            this._Architecture = "x64";
            this._Platform = "core";
            this._Release = "latest";
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora framework OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -f, --framework PATH        xeora framework installation path (required)");
            Console.WriteLine("   -a, --arch (x86|x64)        specify the target CPU architecture otherwise it will be 'x64'");
            Console.WriteLine("   -p, --platf (standard|core) platform specific xeora release otherwise it will download 'core' release");
            Console.WriteLine("   -r, --release VERSION       downloads the specified xeora framework version otherwise it will download 'latest' release");
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
                    case "--framework":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora framework installation path should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._XeoraFrameworkPath = args[aC + 1];
                        aC++;

                        break;
                    case "-a":
                    case "--arch":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("CPU architecture should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Architecture = args[aC + 1];
                        if (Array.IndexOf<string>(this._AvailArchitectures, this._Architecture) == -1)
                        {
                            this.PrintUsage();
                            Console.WriteLine("CPU architecture is not recognized");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    case "-p":
                    case "--platf":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("platform should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Platform = args[aC + 1];
                        if (Array.IndexOf<string>(this._AvailPlatforms, this._Platform) == -1)
                        {
                            this.PrintUsage();
                            Console.WriteLine("platform is not recognized");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    case "-r":
                    case "--release":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora framework version should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Release = args[aC + 1];
                        aC++;

                        break;
                }
            }

            if (string.IsNullOrEmpty(this._XeoraFrameworkPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora framework installation path is required");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public int Execute()
        {
            try
            {
                string requestURL = Framework.XEORAORG;
                requestURL = string.Concat(requestURL, "/", (this._Architecture == "x86" ? "Any" : this._Architecture), "/", this._Platform, "_v", this._Release, "_", this._Architecture, ".zip");

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 15);
                Task<HttpResponseMessage> task = client.GetAsync(requestURL);
                task.Wait();

                if (!task.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine("release has not been found");
                    return 2;
                }

                DirectoryInfo xeoraFrameworkRoot =
                    new DirectoryInfo(this._XeoraFrameworkPath);
                if (!xeoraFrameworkRoot.Exists)
                    xeoraFrameworkRoot.Create();
                Console.WriteLine(string.Format("Downloading Xeora Framework to {0}", xeoraFrameworkRoot.FullName));

                this._LastPercent = -1;
                this.DownloadAsync(xeoraFrameworkRoot, task.Result).Wait();

                Console.WriteLine();
                Console.WriteLine();

                return 0;
            }
            catch (Exception)
            {
                Console.WriteLine("communication problem with server");
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

        private async Task DownloadAsync(DirectoryInfo target, HttpResponseMessage responseMessage)
        {
            FileInfo targetFile = 
                new FileInfo(Path.Combine(target.FullName, "xeora.release.zip"));
            if (targetFile.Exists)
                targetFile.Delete();
            
            long total = responseMessage.Content.Headers.ContentLength.HasValue ? responseMessage.Content.Headers.ContentLength.Value : -1L;

            Stream remoteFS = null;
            Stream targetFS = null;
            try
            {
                targetFS = targetFile.OpenWrite();
                remoteFS = await responseMessage.Content.ReadAsStreamAsync();

                long totalRead = 0L;
                var buffer = new byte[8192];

                do
                {
                    int bR = await remoteFS.ReadAsync(buffer, 0, buffer.Length);

                    if (bR == 0)
                        break;

                    targetFS.Write(buffer, 0, bR);

                    totalRead += bR;

                    this.UpdateProgress(totalRead, total);
                } while (true);
            }
            finally
            {
                if (remoteFS != null)
                    remoteFS.Close();

                if (targetFS != null)
                    targetFS.Close();
            }

            ZipFile.ExtractToDirectory(targetFile.FullName, target.FullName);
            targetFile.Delete();
        }

        private void UpdateProgress(long current, long total)
        {
            if (total == -1)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("\t Downloading... ");

                return;
            }

            long percent = (current * 100) / total;

            if (this._LastPercent != percent)
            {
                this._LastPercent = percent;

                int curPos = Console.CursorLeft;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("\t Downloading... ");
                Console.Write(percent.ToString().PadLeft(3));
                Console.Write("% ");
                for (int pC = 0; pC < this._LastPercent; pC += 2)
                    Console.Write("*");
                if (this._LastPercent == 100)
                    Console.Write(" Completed");
            }
        }
    }
}
