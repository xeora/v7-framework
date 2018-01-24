namespace Xeora.Web.Deployment
{
    internal class DomainFileEntry
    {
        internal DomainFileEntry(long index, string registrationPath, string fileName, long length, long compressedLength)
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
        public string SearchKey => DomainFileEntry.CreateSearchKey(this.RegistrationPath, this.FileName);
        public long Length { get; private set; }
        public long CompressedLength { get; private set; }

        public static string CreateSearchKey(string registrationPath, string fileName) =>
            string.Format("{0}${1}", registrationPath, fileName);
    }
}
