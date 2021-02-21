using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xeora.CLI.Basics
{
    public static class Common
    {
        public static readonly string[] AvailableArchitectures = { "amd64", "arm64" };
        public static readonly string[] AvailableOss = { "windows", "linux", "osx" };
        
        public static string ExecutingArch()
        {
            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.Arm64:
                case Architecture.Arm:
                    return "arm64";
                default:
                    return "amd64";
            }
        }
        
        public static string ExecutingOs()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";

            return "windows";
        }
        
        public static bool CheckArgument(IReadOnlyList<string> argument, int index)
        {
            if (argument.Count <= index + 1)
                return false;

            string value = 
                argument[index + 1];
            
            return value.IndexOf("-", StringComparison.Ordinal) != 0;
        }
    }
}