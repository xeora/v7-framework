using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Xeora.Web.Deployment
{
    internal class Extractor
    {
        private readonly string _DomainFileLocation;
        private readonly byte[] _PasswordHash;

        private readonly ConcurrentDictionary<string, FileEntry> _DomainFileEntryListCache;
        private readonly ConcurrentDictionary<string, byte[]> _DomainFileEntryBytesCache;
        private DateTime _CacheDate;

        public Extractor(string domainRoot)
        {
            this._DomainFileEntryListCache = new ConcurrentDictionary<string, FileEntry>();
            this._DomainFileEntryBytesCache = new ConcurrentDictionary<string, byte[]>();
            this._CacheDate = DateTime.MinValue;
            
            this._DomainFileLocation =
                Path.Combine(domainRoot, "app.xeora");
            string domainPasswordFileLocation =
                Path.Combine(domainRoot, "app.secure");

            this._PasswordHash = 
                this.CreatePasswordHash(domainPasswordFileLocation);

            this.Load();
        }

        private byte[] CreatePasswordHash(string domainPasswordFileLocation)
        {
            if (!File.Exists(domainPasswordFileLocation)) 
                return null;
            
            byte[] securedHash = new byte[16];
            Stream passwordStream = null;
            try
            {
                passwordStream = 
                    new FileStream(domainPasswordFileLocation, FileMode.Open, FileAccess.Read);
                passwordStream.Read(securedHash, 0, securedHash.Length);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                passwordStream?.Close();
            }

            byte[] fileHash;
            Stream contentStream = null;
            try
            {
                contentStream = 
                    new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read);

                MD5CryptoServiceProvider md5 = 
                    new MD5CryptoServiceProvider();
                fileHash = md5.ComputeHash(contentStream);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                contentStream?.Close();
            }

            byte[] passwordHash = new byte[16];

            for (int hC = 0; hC < passwordHash.Length; hC++)
                passwordHash[hC] = (byte)(securedHash[hC] ^ fileHash[hC]);

            return passwordHash;
        }
        
        private void Load()
        {
            FileInfo domainFI =
                new FileInfo(this._DomainFileLocation);

            if (!domainFI.Exists) return;
            if (DateTime.Compare(this._CacheDate, DateTime.MinValue) != 0 &&
                DateTime.Compare(this._CacheDate, domainFI.CreationTime) == 0) return;
            
            this.PrepareFileList();
            this._CacheDate = domainFI.CreationTime;
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

        public IEnumerable<FileEntry> Search(string registrationPath, string fileName)
        {
            List<FileEntry> result = new List<FileEntry>();

            string[] keys = new string[this._DomainFileEntryListCache.Count];
            this._DomainFileEntryListCache.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                if (key.IndexOf(
                        FileEntry.CreateSearchKey(registrationPath, fileName), StringComparison.Ordinal) != 0) continue;
                
                if (this._DomainFileEntryListCache.TryGetValue(key, out FileEntry fileEntry))
                    result.Add(fileEntry);
            }

            return result.ToArray();
        }

        public RequestResults Read(long index, long length, ref Stream outputStream)
        {
            if (index == -1)
                throw new IndexOutOfRangeException();
            if (length < 0)
                throw new ArgumentOutOfRangeException();
            if (outputStream == null)
                throw new NullReferenceException();

            // Search in Cache First
            string searchKey =
                $"{this._DomainFileLocation}$i:{index}.l:{length}";

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
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = (byte)(buffer[i] ^ this._PasswordHash[i % this._PasswordHash.Length]);
                // !--

                gzipHelperStream = new MemoryStream(buffer, 0, buffer.Length, false);
                gzipCStream = new GZipStream(gzipHelperStream, CompressionMode.Decompress, false);

                byte[] rBuffer = new byte[512];
                int bC, total = 0;

                do
                {
                    bC = gzipCStream.Read(rBuffer, 0, rBuffer.Length);
                    total += bC;

                    if (bC > 0)
                        outputStream.Write(rBuffer, 0, bC);
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
            catch (Exception)
            {
                return RequestResults.PasswordError;
            }
            finally
            {
                domainFileStream?.Close();
                gzipCStream?.Close();
                gzipHelperStream?.Close();
            }
        }

        private void PrepareFileList()
        {
            this._DomainFileEntryListCache.Clear();
            this._DomainFileEntryBytesCache.Clear();

            BinaryReader fileReader = null;
            try
            {
                Stream fileStream = 
                    new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileReader = new BinaryReader(fileStream, Encoding.UTF8);

                int readC = 0;
                long movedIndex = fileReader.ReadInt64();

                do
                {
                    long indexTotal = fileReader.BaseStream.Position;

                    long index = fileReader.ReadInt64() + movedIndex + 8;
                    string localRegistrationPath = fileReader.ReadString();
                    string localFileName = fileReader.ReadString();
                    long length = fileReader.ReadInt64();
                    long compressedLength = fileReader.ReadInt64();

                    readC += (int)(fileReader.BaseStream.Position - indexTotal);

                    FileEntry fileEntry =
                        new FileEntry(index, localRegistrationPath, localFileName, length, compressedLength);

                    this._DomainFileEntryListCache.TryAdd(fileEntry.SearchKey, fileEntry);
                } while (readC != movedIndex);
            }
            finally
            {
                fileReader?.Close();
            }
        }
    }
}
