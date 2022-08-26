using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Xeora.CLI.Basics
{
    public class Framework : ICommand
    {
        private const string XEORA_ORG = "https://xeora.org/Releases/v7";

        private string _Architecture;
        private string _Os;
        private string _Release;

        private long _LastPercent;

        public Framework()
        {
            this._Architecture = Common.ExecutingArch();
            this._Os = Common.ExecutingOs();
            this._Release = "latest";
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora framework OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                    print this screen");
            Console.WriteLine($"   -a, --arch (amd64|arm64)      specify the target CPU architecture otherwise it will be '{Common.ExecutingArch()}'");
            Console.WriteLine($"   -o, --os (windows|linux|osx)  specify the target OS otherwise it will be '{Common.ExecutingOs()}'");
            Console.WriteLine("   -r, --release VERSION         downloads the specified xeora framework version otherwise it will download 'latest' release");
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
                    case "-a":
                    case "--arch":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("CPU architecture should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Architecture = args[aC + 1];
                        if (Array.IndexOf(Common.AvailableArchitectures, this._Architecture) == -1)
                        {
                            this.PrintUsage();
                            Console.WriteLine("CPU architecture is not recognized");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    case "-o":
                    case "--os":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("OS should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Os = args[aC + 1];
                        if (Array.IndexOf(Common.AvailableOss, this._Os) == -1)
                        {
                            this.PrintUsage();
                            Console.WriteLine("OS is not recognized");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    case "-r":
                    case "--release":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora framework version should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._Release = args[aC + 1];
                        aC++;

                        break;
                    default:
                        this.PrintUsage();
                        Console.WriteLine("unrecognizable argument");
                        Console.WriteLine();
                        return 2;
                }
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
                string requestUrl =
                    string.Concat(Framework.XEORA_ORG, "/", this._Architecture, "/core_v", this._Release, "_", this._Os ,"_", this._Architecture, ".zip");

                HttpClient client = 
                    new HttpClient {Timeout = new TimeSpan(0, 0, 15)};
                HttpResponseMessage response = 
                    await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("release has not been found");
                    return 2;
                }

                string xeoraFramework =
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Console.WriteLine($"Downloading Xeora Framework to {xeoraFramework}");

                this._LastPercent = -1;
                this.DownloadAsync(xeoraFramework, response).Wait();

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

        private async Task DownloadAsync(string target, HttpResponseMessage responseMessage)
        {
            FileInfo targetFile = 
                new FileInfo(Path.Combine(Path.GetTempPath(), "xeora.release.zip"));
            if (targetFile.Exists) targetFile.Delete();
            
            long total = responseMessage.Content.Headers.ContentLength ?? -1L;

            Stream remoteFs = null;
            Stream targetFs = null;
            try
            {
                targetFs = 
                    targetFile.OpenWrite();
                remoteFs = await responseMessage.Content.ReadAsStreamAsync();

                long totalRead = 0L;
                var buffer = new byte[8192];

                do
                {
                    int bR = await remoteFs.ReadAsync(buffer.AsMemory(0, buffer.Length)); 
                    if (bR == 0) break;

                    await targetFs.WriteAsync(buffer.AsMemory(0, bR));

                    totalRead += bR;

                    this.UpdateProgress(totalRead, total);
                } while (true);
            }
            finally
            {
                remoteFs?.Dispose();
                targetFs?.Dispose();
            }

            Console.Write("   Extracting...  ");
            try
            {
                ZipFile.ExtractToDirectory(targetFile.FullName, target, true);
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed! {e.Message}");
                return;
            }
            
            try
            {
                targetFile.Delete();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UpdateProgress(long current, long total)
        {
            if (total == -1)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("   Downloading... ");

                return;
            }

            long percent = 
                current * 100 / total;
            if (this._LastPercent == percent) return;
            
            this._LastPercent = percent;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("   Downloading... ");
            Console.Write(percent.ToString().PadLeft(3));
            Console.Write("% ");
            for (int pC = 0; pC < this._LastPercent; pC += 2)
                Console.Write("*");
            if (this._LastPercent == 100)
                Console.WriteLine(" Completed");
        }
    }
}
