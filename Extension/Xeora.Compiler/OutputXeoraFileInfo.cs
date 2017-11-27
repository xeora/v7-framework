namespace Xeora.Extension.Tools
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

        public long Index { get; private set; }
        public string RegistrationPath { get; private set; }
        public string FileName { get; private set; }
        public long Length { get; private set; }
        public long CompressedLength { get; private set; }
    }
}
