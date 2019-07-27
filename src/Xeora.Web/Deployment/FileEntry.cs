namespace Xeora.Web.Deployment
{
    internal class FileEntry
    {
        internal FileEntry(long index, string registrationPath, string fileName, long length, long compressedLength)
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
        public string SearchKey => FileEntry.CreateSearchKey(this.RegistrationPath, this.FileName);
        public long Length { get; }
        public long CompressedLength { get; }

        public static string CreateSearchKey(string registrationPath, string fileName) =>
            $"{registrationPath}${fileName}";
    }
}
