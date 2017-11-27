using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Xeora.Extension.Tools
{
    public class Compiler
    {
        public event ProgressEventHandler Progress;
        public delegate void ProgressEventHandler(int current, int total);

        private string _DomainPath;
        private List<InputXeoraFileInfo> _XeoraFiles;
        private byte[] _SecuredPasswordHash = null;

        public Compiler(string domainPath)
        {
            this._DomainPath = domainPath;
            this._XeoraFiles = new List<InputXeoraFileInfo>();
        }

        public byte[] PasswordHash =>
            this._SecuredPasswordHash;

        public void AddFile(string fullFilePath) =>
            this._XeoraFiles.Add(new InputXeoraFileInfo(this._DomainPath, fullFilePath));

        public void RemoveFile(string fullFilePath)
        {
            InputXeoraFileInfo inputXeoraFileInfo =
                new InputXeoraFileInfo(this._DomainPath, fullFilePath);

            for (int xFC = this._XeoraFiles.Count - 1; xFC >= 0; xFC--)
            {
                if (this._XeoraFiles[xFC].GetHashCode() == inputXeoraFileInfo.GetHashCode())
                {
                    this._XeoraFiles.RemoveAt(xFC);

                    break;
                }
            }
        }

        public void CreateDomainFile(ref Stream outputStream) =>
            this.CreateDomainFile(null, ref outputStream);

        public void CreateDomainFile(string password, ref Stream outputStream)
        {
            if (this._XeoraFiles.Count == 0)
                throw new Exception("File List must not be empty!");
            if (outputStream == null)
                throw new Exception("Output Stream must be defined!");

            System.Security.Cryptography.MD5CryptoServiceProvider md5 = null;
            byte[] passwordHash = null;

            if (!string.IsNullOrEmpty(password))
            {
                md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                passwordHash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            // 1 For File Preperation
            // 1 For Index Creating
            // Total 2
            Progress(0, this._XeoraFiles.Count + 2);

            // Compiler Streams
            Stream contentPartStream = null;
            Stream contentPartFileStream = null;
            GZipStream gzipStream = null;

            Stream indexStream = new MemoryStream();
            BinaryWriter indexBinaryWriter =
                new BinaryWriter(indexStream, System.Text.Encoding.UTF8);
            Stream contentStream = new MemoryStream();
            // !--

            // Helper Variables
            int rC = 0;
            byte[] buffer = new byte[512];

            int eC = 1;
            // !--

            foreach (InputXeoraFileInfo xFI in this._XeoraFiles)
            {
                contentPartStream = new MemoryStream();

                contentPartFileStream =
                    new FileStream(xFI.FullFilePath, FileMode.Open, FileAccess.Read);
                gzipStream =
                    new GZipStream(contentPartStream, CompressionMode.Compress, true);

                do
                {
                    rC = contentPartFileStream.Read(buffer, 0, buffer.Length);

                    if (rC > 0)
                        gzipStream.Write(buffer, 0, rC);
                } while (rC != 0);

                gzipStream.Flush();
                gzipStream.Close();
                GC.SuppressFinalize(gzipStream);

                contentPartFileStream.Close();
                GC.SuppressFinalize(contentPartFileStream);

                // CREATE INDEX
                // Write Index Info
                indexBinaryWriter.Write(contentStream.Position);

                // Write RegistrationPath
                indexBinaryWriter.Write(xFI.RegistrationPath);

                // Write FileName
                indexBinaryWriter.Write(xFI.FileName);

                // Write Original Size
                indexBinaryWriter.Write(xFI.FileSize);

                // Write Compressed Size
                indexBinaryWriter.Write(contentPartStream.Length);

                // Flush to Underlying Stream
                indexBinaryWriter.Flush();

                // !--

                // PROTECT FILE
                if (passwordHash != null)
                {
                    contentPartStream.Seek(0, SeekOrigin.Begin);

                    int lastIndex = 0;
                    do
                    {
                        rC = contentPartStream.Read(buffer, 0, buffer.Length);

                        if (rC > 0)
                        {
                            contentPartStream.Seek(-rC, SeekOrigin.Current);

                            for (int bC = 0; bC < rC; bC++)
                                contentPartStream.WriteByte((byte)(buffer[bC] ^ passwordHash[(bC + lastIndex) % passwordHash.Length]));

                            lastIndex += rC;
                        }
                    } while (rC != 0);
                }
                // !--

                // WRITE CONTENT
                contentPartStream.Seek(0, SeekOrigin.Begin);
                do
                {
                    rC = contentPartStream.Read(buffer, 0, buffer.Length);

                    if (rC > 0)
                        contentStream.Write(buffer, 0, rC);
                } while (rC != 0);
                // !--

                contentPartStream.Close();
                GC.SuppressFinalize(contentPartStream);

                Progress(eC, this._XeoraFiles.Count + 2);

                eC += 1;
            }

            BinaryWriter outputBinaryWriter = new BinaryWriter(outputStream);

            // Write Index Length
            outputBinaryWriter.Write(indexStream.Position);

            // Write Index Content
            indexStream.Seek(0, SeekOrigin.Begin);
            do
            {
                rC = indexStream.Read(buffer, 0, buffer.Length);

                if (rC > 0)
                    outputStream.Write(buffer, 0, rC);
            } while (rC != 0);
            // !--

            outputBinaryWriter.Flush();

            // Write Content
            contentStream.Seek(0, SeekOrigin.Begin);
            do
            {
                rC = contentStream.Read(buffer, 0, buffer.Length);

                if (rC > 0)
                    outputStream.Write(buffer, 0, rC);
            } while (rC != 0);
            // !--

            indexBinaryWriter.Close();
            GC.SuppressFinalize(indexBinaryWriter);

            contentStream.Close();
            GC.SuppressFinalize(contentStream);

            Progress(eC + 1, this._XeoraFiles.Count + 2);

            if (passwordHash != null)
            {
                outputStream.Flush();
                outputStream.Seek(0, SeekOrigin.Begin);

                byte[] FileHash = 
                    md5.ComputeHash(outputStream);
                this._SecuredPasswordHash = new byte[FileHash.Length];

                for (int hC = 0; hC < FileHash.Length; hC++)
                    this._SecuredPasswordHash[hC] = (byte)(FileHash[hC] ^ passwordHash[hC]);
            }
        }
    }
}
