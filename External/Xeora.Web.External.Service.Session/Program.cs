using System;
using System.Net;

namespace Xeora.Web.External.Service.Session
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IPAddress ipAddress = null;
            short ipPort = 0;

            for (int aC = 0; aC < args.Length; aC++)
            {
                switch(args[aC])
                {
                    case "-i":
                        if (!IPAddress.TryParse(args[aC+1], out ipAddress))
                        {
                            Program.PrintUsage();
                            Environment.Exit(1);
                        }
                        aC++;

                        break;
                    case "-p":
                        if (!short.TryParse(args[aC + 1], out ipPort))
                        {
                            Program.PrintUsage();
                            Environment.Exit(1);
                        }
                        aC++;

                        break;
                }
            }

            if (ipAddress == null)
                ipAddress = IPAddress.Parse("127.0.0.1");
            if (ipPort == 0)
                ipPort = 5531;

            SessionServer sessionServer = new SessionServer(new IPEndPoint(ipAddress, ipPort));
            int exitCode = sessionServer.Start();

            Environment.Exit(exitCode);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Xeora Session Service");
            Console.WriteLine("Usage: -i 127.0.0.1 -p 5531");
        }
    }
}
