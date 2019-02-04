using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Xeora.Web.Deployment
{
    internal class Decompiler
    {
        private readonly string _DomainFileLocation;
        private readonly byte[] _PasswordHash = null;

        private readonly ConcurrentDictionary<string, FileEntry> _DomainFileEntryListCache;
        private readonly ConcurrentDictionary<string, byte[]> _DomainFileEntryBytesCache;
        private DateTime _CacheDate;

        public Decompiler(string domainRoot)
        {
            this._DomainFileEntryListCache = new ConcurrentDictionary<string, FileEntry>();
            this._DomainFileEntryBytesCache = new ConcurrentDictionary<string, byte[]>();
            this._CacheDate = DateTime.MinValue;

            this._DomainFileLocation =
                Path.Combine(domainRoot, "Content.xeora");
            string domainPasswordFileLocation =
                Path.Combine(domainRoot, "Content.secure");

            if (File.Exists(domainPasswordFileLocation))
            {
                this._PasswordHash = null;

                byte[] securedHash = new byte[16];
                Stream passwordFS = null;
                try
                {
                    passwordFS = new FileStream(domainPasswordFileLocation, FileMode.Open, FileAccess.Read);
                    passwordFS.Read(securedHash, 0, securedHash.Length);
                }
                catch (System.Exception)
                {
                    securedHash = null;
                }
                finally
                {
                    if (passwordFS != null)
                        passwordFS.Close();
                }

                byte[] fileHash = null;
                Stream contentFS = null;
                try
                {
                    contentFS = new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read);

                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    fileHash = md5.ComputeHash(contentFS);
                }
                catch (System.Exception)
                {
                    fileHash = null;
                }
                finally
                {
                    if (contentFS != null)
                        contentFS.Close();
                }

                if (securedHash != null && (fileHash != null))
                {
                    this._PasswordHash = new byte[16];

                    for (int hC = 0; hC < this._PasswordHash.Length; hC++)
                        this._PasswordHash[hC] = (byte)(securedHash[hC] ^ fileHash[hC]);
                }
            }

            this.Reload();
        }

        public FileEntry Get(string registrationPath, string fileName)
        {
            // Search In Cache First
            string cacheSearchKey =
                FileEntry.CreateSearchKey(registrationPath, fileName);

            if (this._DomainFileEntryListCache.TryGetValue(cacheSearchKey, out FileEntry fileEntry))
                return fileEntry;
            // !---

            return new FileEntry(-1, null, null, -1, -1);
        }

        public FileEntry[] Search(string registrationPath, string fileName)
        {
            List<FileEntry> result = new List<FileEntry>();

            string[] keys = new string[this._DomainFileEntryListCache.Count];
            this._DomainFileEntryListCache.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                if (key.IndexOf(
                        FileEntry.CreateSearchKey(registrationPath, fileName)) == 0)
                {
                    if (this._DomainFileEntryListCache.TryGetValue(key, out FileEntry fileEntry))
                        result.Add(fileEntry);
                }
            }

            return result.ToArray();
        }

        public RequestResults Read(long index, long length, ref Stream outputStream)
        {
            if (index == -1)
                throw new IndexOutOfRangeException();
            if (length < 1)
                throw new ArgumentOutOfRangeException();
            if (outputStream == null)
                throw new NullReferenceException();

            // Search in Cache First
            string searchKey =
                string.Format("{0}$i:{1}.l:{2}", this._DomainFileLocation, index, length);

            if (this._DomainFileEntryBytesCache.TryGetValue(searchKey, out byte[] buffer))
            {
                outputStream.Write(buffer, 0, buffer.Length);
                outputStream.Seek(0, SeekOrigin.Begin);

                return RequestResults.Authenticated;
            }
            // !---

            Stream domainFileStream = null;
            Stream gzipHelperStream = null;
            GZipStream gzipCStream = null;

            buffer = new byte[length];
            try
            {
                domainFileStream =
                    new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                domainFileStream.Seek(index, SeekOrigin.Begin);
                domainFileStream.Read(buffer, 0, buffer.Length);

                // FILE PROTECTION
                if (this._PasswordHash != null)
                    for (int pBC = 0; pBC < buffer.Length; pBC++)
                        buffer[pBC] = (byte)(buffer[pBC] ^ this._PasswordHash[pBC % this._PasswordHash.Length]);
                // !--

                gzipHelperStream = new MemoryStream(buffer, 0, buffer.Length, false);
                gzipCStream = new GZipStream(gzipHelperStream, CompressionMode.Decompress, false);

                byte[] rbuffer = new byte[512];
                int bC = 0, total = 0;

                do
                {
                    bC = gzipCStream.Read(rbuffer, 0, rbuffer.Length);
                    total += bC;

                    if (bC > 0)
                        outputStream.Write(rbuffer, 0, bC);
                } while (bC > 0);

                // Cache What You Read
                byte[] cacheBytes = new byte[total];

                outputStream.Seek(0, SeekOrigin.Begin);
                outputStream.Read(cacheBytes, 0, cacheBytes.Length);

                this._DomainFileEntryBytesCache.AddOrUpdate(searchKey, cacheBytes, (cKey, cValue) => cacheBytes);
                // !---

                outputStream.Seek(0, SeekOrigin.Begin);

                return RequestResults.Authenticated;
            }
            catch (FileNotFoundException)
            {
                return RequestResults.ContentNotExists;
            }
            catch (System.Exception)
            {
                return RequestResults.PasswordError;
            }
            finally
            {
                if (domainFileStream != null)
                {
                    domainFileStream.Close();
                    GC.SuppressFinalize(domainFileStream);
                }

                if (gzipCStream != null)
                {
                    gzipCStream.Close();
                    GC.SuppressFinalize(gzipCStream);
                }

                if (gzipHelperStream != null)
                {
                    gzipHelperStream.Close();
                    GC.SuppressFinalize(gzipHelperStream);
                }
            }
        }

        public bool Reload()
        {
            FileInfo domainFI =
                new FileInfo(this._DomainFileLocation);

            if (domainFI.Exists)
            {
                if (DateTime.Compare(this._CacheDate, DateTime.MinValue) == 0 ||
                    DateTime.Compare(this._CacheDate, domainFI.CreationTime) != 0)
                {
                    this.PrepareFileList();
                    this._CacheDate = domainFI.CreationTime;

                    return true;
                }
            }

            return false;
        }

        private void PrepareFileList()
        {
            this._DomainFileEntryListCache.Clear();
            this._DomainFileEntryBytesCache.Clear();

            long index = -1, length = -1, compressedLength = -1;
            string localRegistrationPath = null, localFileName = null;

            Stream fileStream = null;
            BinaryReader fileReader = null;
            try
            {
                fileStream =
                    new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileReader = new BinaryReader(fileStream, Encoding.UTF8);

                int readC = 0;
                long indexTotal = 0, movedIndex = fileReader.ReadInt64();

                do
                {
                    indexTotal = fileReader.BaseStream.Position;

                    index = fileReader.ReadInt64() + movedIndex + 8;
                    localRegistrationPath = fileReader.ReadString();
                    localFileName = fileReader.ReadString();
                    length = fileReader.ReadInt64();
                    compressedLength = fileReader.ReadInt64();

                    readC += (int)(fileReader.BaseStream.Position - indexTotal);

                    FileEntry fileEntry =
                        new FileEntry(index, localRegistrationPath, localFileName, length, compressedLength);

                    this._DomainFileEntryListCache.TryAdd(fileEntry.SearchKey, fileEntry);
                } while (readC != movedIndex);
            }
            finally
            {
                if (fileReader != null)
                {
                    fileReader.Close();
                    GC.SuppressFinalize(fileReader);
                }
            }
        }
    }
}
