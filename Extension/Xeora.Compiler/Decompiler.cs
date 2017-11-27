using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Xeora.Extension.Tools
{
    public class Decompiler
    {
        private string _XeoraFileLocation;
        private byte[] _PasswordHash = null;

        public Decompiler(string xeoraFileLocation, byte[] password)
        {
            this._XeoraFileLocation = xeoraFileLocation;
            this._PasswordHash = password;

            this.Authenticated = false;
        }

        public bool Authenticated { get; private set; }

        public List<OutputXeoraFileInfo> XeoraFilesList =>
            this.ReadFileListInternal();

        private List<OutputXeoraFileInfo> ReadFileListInternal()
        {
            List<OutputXeoraFileInfo> rXeoraFileInfoList = new List<OutputXeoraFileInfo>();

            long index = -1;
            string localRegistrationPath = null;
            string localFileName = null;
            long length = -1;
            long compressedLength = -1;

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
                    index = xeoraStreamBinaryReader.ReadInt64() + movedIndex;
                    localRegistrationPath = xeoraStreamBinaryReader.ReadString();
                    localFileName = xeoraStreamBinaryReader.ReadString();
                    length = xeoraStreamBinaryReader.ReadInt64();
                    compressedLength = xeoraStreamBinaryReader.ReadInt64();

                    readC += 8 + localRegistrationPath.Length + localFileName.Length + 8 + 8;

                    rXeoraFileInfoList.Add(new OutputXeoraFileInfo(index, localRegistrationPath, localFileName, length, compressedLength));
                } while (readC != movedIndex);
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
            finally
            {
                if (xeoraStreamBinaryReader != null)
                {
                    xeoraStreamBinaryReader.Close();
                    GC.SuppressFinalize(xeoraStreamBinaryReader);
                }
            }

            return rXeoraFileInfoList;
        }

        public OutputXeoraFileInfo GetXeoraFileInfo(string registrationPath, string fileName)
        {
            long index = -1;
            string localRegistrationPath = null;
            string localFileName = null;
            long length = -1;
            long compressedLength = -1;

            Stream xeoraFileStream = null;
            BinaryReader xeoraStreamBinaryReader = null;
            bool isFound = false;
            try
            {
                xeoraFileStream = 
                    new FileStream(this._XeoraFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                xeoraStreamBinaryReader = new BinaryReader(xeoraFileStream, System.Text.Encoding.UTF8);

                int readC = 0;
                long movedIndex = xeoraStreamBinaryReader.ReadInt64();

                do
                {
                    index = xeoraStreamBinaryReader.ReadInt64() + movedIndex + 8;
                    localRegistrationPath = xeoraStreamBinaryReader.ReadString();
                    localFileName = xeoraStreamBinaryReader.ReadString();
                    length = xeoraStreamBinaryReader.ReadInt64();
                    compressedLength = xeoraStreamBinaryReader.ReadInt64();

                    readC += 8 + (1 + localRegistrationPath.Length) + (1 + localFileName.Length) + 8 + 8;

                    if (string.Compare(registrationPath, localRegistrationPath, true) == 0 && 
                        string.Compare(fileName, localFileName, true) == 0)
                    {
                        isFound = true;

                        break;
                    }
                } while (readC != movedIndex);
            }
            catch (Exception)
            {
                isFound = false;
            }
            finally
            {
                if (xeoraStreamBinaryReader != null)
                {
                    xeoraStreamBinaryReader.Close();
                    GC.SuppressFinalize(xeoraStreamBinaryReader);
                }
            }

            if (!isFound)
            {
                index = -1;
                localRegistrationPath = null;
                localFileName = null;
                length = -1;
                compressedLength = -1;
            }

            return new OutputXeoraFileInfo(index, localRegistrationPath, localFileName, length, compressedLength);
        }

        public void ReadFile(long index, long length, ref Stream outputStream)
        {
            if (index == -1)
                throw new Exception("Index must be specified!");
            if (length < 1)
                throw new Exception("Length must be specified!");
            if (outputStream == null)
                throw new Exception("OutputStream must be specified!");

            Stream xeoraFileStream = null;
            Stream gzipHelperStream = null;
            GZipStream gzipStream = null;

            byte[] buffer = new byte[length];

            try
            {
                xeoraFileStream = new FileStream(this._XeoraFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                xeoraFileStream.Seek(index, SeekOrigin.Begin);
                xeoraFileStream.Read(buffer, 0, buffer.Length);

                // FILE PROTECTION
                if (this._PasswordHash != null)
                {
                    for (int pBC = 0; pBC < buffer.Length; pBC++)
                        buffer[pBC] = (byte)(buffer[pBC] ^ this._PasswordHash[pBC % this._PasswordHash.Length]);
                }
                // !--

                gzipHelperStream = new MemoryStream(buffer, 0, buffer.Length, false);
                gzipStream = new GZipStream(gzipHelperStream, CompressionMode.Decompress, false);

                byte[] rbuffer = new byte[512];
                int bC = 0;

                do
                {
                    bC = gzipStream.Read(rbuffer, 0, rbuffer.Length);

                    if (bC > 0)
                        outputStream.Write(rbuffer, 0, bC);
                } while (bC > 0);

                this.Authenticated = true;
            }
            catch (Exception)
            {
                this.Authenticated = false;
            }
            finally
            {
                if (xeoraFileStream != null)
                {
                    xeoraFileStream.Close();
                    GC.SuppressFinalize(xeoraFileStream);
                }

                if (gzipStream != null)
                {
                    gzipStream.Close();
                    GC.SuppressFinalize(gzipStream);
                }

                if (gzipHelperStream != null)
                {
                    gzipHelperStream.Close();
                    GC.SuppressFinalize(gzipHelperStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
