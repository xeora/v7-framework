using System.IO;

namespace Xeora.CLI.Extensions.Build
{
    public class InputXeoraFileInfo
    {
        public InputXeoraFileInfo(string domainPath, string fullFilePath)
        {
            FileInfo fI = new FileInfo(fullFilePath);

            this.RegistrationPath = fI.FullName;
            this.RegistrationPath = 
                this.RegistrationPath.Replace(domainPath, string.Empty);
            this.RegistrationPath = 
                this.RegistrationPath.Replace(fI.Name, string.Empty);
            this.RegistrationPath =
                this.RegistrationPath.Replace("/", "\\");
            this.FileName = fI.Name;

            this.FullFilePath = fullFilePath;
            this.FileSize = fI.Length;
        }

        public string RegistrationPath { get; }
        public string FileName { get; }
        public string FullFilePath { get; }
        public long FileSize { get; }
    }
}
