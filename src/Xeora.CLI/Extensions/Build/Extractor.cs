using System;
using System.IO;
using System.IO.Compression;

namespace Xeora.CLI.Extensions.Build
{
    public class Extractor
    {
        private readonly string _XeoraFileLocation;
        private readonly byte[] _PasswordHash;

        public Extractor(string xeoraFileLocation, byte[] password)
        {
            this._XeoraFileLocation = xeoraFileLocation;
            this._PasswordHash = password;
        }

        public void QueryList(Func<OutputXeoraFileInfo, bool> fileHandler)
        {
            Stream xeoraFileStream = null;
            BinaryReader xeoraStreamBinaryReader = null;
            try
            {
                xeoraFileStream = 
                    new FileStream(this._XeoraFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                xeoraStreamBinaryReader = new BinaryReader(xeoraFileStream, System.Text.Encoding.UTF8);

                int readC = 0;
                long movedIndex = xeoraStreamBinaryReader.ReadInt64();

                do
                {
                    long indexTotal = xeoraStreamBinaryReader.BaseStream.Position;
                    
                    long index = 
                        xeoraStreamBinaryReader.ReadInt64() + movedIndex + 8;
                    string localRegistrationPath = 
                        xeoraStreamBinaryReader.ReadString();
                    string localFileName = 
                        xeoraStreamBinaryReader.ReadString();
                    long length = 
                        xeoraStreamBinaryReader.ReadInt64();
                    long compressedLength = 
                        xeoraStreamBinaryReader.ReadInt64();

                    readC += (int)(xeoraStreamBinaryReader.BaseStream.Position - indexTotal);

                    if (!fileHandler.Invoke(new OutputXeoraFileInfo(index, localRegistrationPath, localFileName, length, compressedLength))) return;
                } while (readC != movedIndex);
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
            finally
            {
                xeoraStreamBinaryReader?.Close();
                xeoraFileStream?.Close();
            }
        }

        public void Read(long index, long length, ref Stream outputStream)
        {
            if (index == -1)
                throw new Exception("Index must be specified!");
            if (length < 0)
                throw new Exception("Length must be specified!");
            if (outputStream == null)
                throw new Exception("OutputStream must be specified!");

            Stream xeoraFileStream = null;
            Stream gzipHelperStream = null;
            GZipStream gzipStream = null;

            byte[] buffer = new byte[length];

            try
            {
                xeoraFileStream = 
                    new FileStream(this._XeoraFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                xeoraFileStream.Seek(index, SeekOrigin.Begin);
                xeoraFileStream.Read(buffer, 0, buffer.Length);

                // FILE PROTECTION
                if (this._PasswordHash != null)
                {
                    for (int pBC = 0; pBC < buffer.Length; pBC++)
                        buffer[pBC] = (byte)(buffer[pBC] ^ this._PasswordHash[pBC % this._PasswordHash.Length]);
                }
                // !--

                gzipHelperStream = 
                    new MemoryStream(buffer, 0, buffer.Length, false);
                gzipStream = 
                    new GZipStream(gzipHelperStream, CompressionMode.Decompress, false);

                byte[] rBuffer = new byte[512];
                int bC;

                do
                {
                    bC = gzipStream.Read(rBuffer, 0, rBuffer.Length);

                    if (bC > 0)
                        outputStream.Write(rBuffer, 0, bC);
                } while (bC > 0);
            }
            finally
            {
                xeoraFileStream?.Close();
                gzipStream?.Close();
                gzipHelperStream?.Close();
            }

            outputStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
