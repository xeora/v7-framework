namespace Xeora.CLI.Extensions.Build
{
    public class OutputXeoraFileInfo
    {
        public OutputXeoraFileInfo(long index, string registrationPath, string fileName, long length, long compressedLength)
        {
            this.Index = index;
            this.RegistrationPath = registrationPath;
            this.FileName = fileName;
            this.Length = length;
            this.CompressedLength = compressedLength;
        }

        public long Index { get; }
        public string RegistrationPath { get; }
        public string FileName { get; }
        public long Length { get; }
        public long CompressedLength { get; }
    }
}
