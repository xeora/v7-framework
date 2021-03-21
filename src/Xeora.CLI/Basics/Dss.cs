using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xeora.Web.Service.Dss;

namespace Xeora.CLI.Basics
{
    public class Dss : ICommand
    {
        private IPAddress _IpAddress;
        private short _Port;

        public Dss()
        {
            this._IpAddress = IPAddress.Parse("127.0.0.1");
            this._Port = 5531;
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora dss OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -i, --ip IPADDRESS          ip address to listen (Default: 127.0.0.1)");
            Console.WriteLine("   -p, --port PORTNUMBER       port number to listen (Default: 5531)");
            Console.WriteLine();
        }

        private int SetArguments(IReadOnlyList<string> args)
        {
            IPAddress ipAddress = null;
            short port = 0;
            
            for (int aC = 0; aC < args.Count; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-i":
                    case "--ip":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("ip address should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        if (!IPAddress.TryParse(args[aC+1], out ipAddress))
                        {
                            this.PrintUsage();
                            Console.WriteLine("ip address is not in a correct format");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    case "-p":
                    case "--port":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("port should be specified");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        if (!short.TryParse(args[aC + 1], out port) || port == 0)
                        {
                            this.PrintUsage();
                            Console.WriteLine("port is not a number or not in a correct range");
                            Console.WriteLine();
                            return 2;
                        }
                        aC++;

                        break;
                    default:
                        if (aC + 1 < args.Count)
                        {
                            this.PrintUsage();
                            Console.WriteLine("unrecognizable argument");
                            Console.WriteLine();
                            return 2;
                        }
                        
                        break;
                }
            }

            if (ipAddress != null)
                this._IpAddress = IPAddress.Parse("127.0.0.1");
            if (port > 0)
                this._Port = 5531;
            
            return 0;
        }

        public async Task<int> Execute(IReadOnlyList<string> args)
        {
            int argumentsResult =
                this.SetArguments(args);
            if (argumentsResult != 0) return argumentsResult;
            
            try
            {
                Server server = 
                    new Server(new IPEndPoint(this._IpAddress, this._Port));
                return await server.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"XeoraDss execution problem: {e.Message}");
                return 1;
            }
        }
    }
}
