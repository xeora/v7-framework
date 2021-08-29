using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        
        public static async Task Copy(DirectoryInfo source, DirectoryInfo target)
        {
            foreach(DirectoryInfo item in source.GetDirectories())
            {
                DirectoryInfo targetItem = 
                    new DirectoryInfo(Path.Combine(target.FullName, item.Name));
                if (!targetItem.Exists) targetItem.Create();

                await Copy(item, targetItem);
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
        
        public static void WriteUpdateToConsole(string action, string sourcePath, string targetPath)
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