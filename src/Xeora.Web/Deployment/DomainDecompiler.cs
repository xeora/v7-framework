using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Xeora.Web.Deployment
{
    internal class DomainDecompiler
    {
        private string _DomainFileLocation;
        private byte[] _PasswordHash = null;

        private static ConcurrentDictionary<string, Dictionary<string, DomainFileEntry>> _DomainFileEntryListCache =
            new ConcurrentDictionary<string, Dictionary<string, DomainFileEntry>>();
        private static Hashtable _DomainFileEntryBytesCache =
            Hashtable.Synchronized(new Hashtable());
        private static ConcurrentDictionary<string, DateTime> _DomainFileEntryLastModifiedDate =
            new ConcurrentDictionary<string, DateTime>();

        public enum RequestResults
        {
            None,
            Authenticated,
            PasswordError,
            ContentNotExists
        }

        public DomainDecompiler(string domainRoot)
        {
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

            FileInfo fI = new FileInfo(this._DomainFileLocation);

            if (fI.Exists)
                DomainDecompiler._DomainFileEntryLastModifiedDate.TryAdd(this._DomainFileLocation, fI.CreationTime);
        }

        public Dictionary<string, DomainFileEntry> FilesList
        {
            get
            {
                // Control Template File Changes
                DateTime cachedFileDate;
                if (!DomainDecompiler._DomainFileEntryLastModifiedDate.TryGetValue(this._DomainFileLocation, out cachedFileDate))
                    cachedFileDate = DateTime.MinValue;

                FileInfo fI = new FileInfo(this._DomainFileLocation);

                if (fI.Exists && DateTime.Compare(cachedFileDate, fI.CreationTime) != 0)
                    this.ClearCache();
                // !---

                Dictionary<string, DomainFileEntry> rFileList;

                if (!DomainDecompiler._DomainFileEntryListCache.TryGetValue(this._DomainFileLocation, out rFileList))
                {
                    rFileList = new Dictionary<string, DomainFileEntry>();

                    DomainFileEntry[] fileEntryList = this.ReadFileList();

                    foreach (DomainFileEntry fileEntry in fileEntryList)
                        rFileList.Add(fileEntry.SearchKey, fileEntry);

                    DomainDecompiler._DomainFileEntryListCache.TryAdd(this._DomainFileLocation, rFileList);
                }

                return rFileList;
            }
        }

        private DomainFileEntry[] ReadFileList()
        {
            List<DomainFileEntry> rFileEntryList = new List<DomainFileEntry>();

            long index = -1, length = -1, compressedLength = -1;
            string localRegistrationPath = null, localFileName = null;

            Stream fileStream = null;
            BinaryReader fileReader = null;
            try
            {
                fileStream = new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

                    rFileEntryList.Add(
                        new DomainFileEntry(index, localRegistrationPath, localFileName, length, compressedLength));
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

            return rFileEntryList.ToArray();
        }

        public DomainFileEntry GetFileEntry(string registrationPath, string fileName)
        {
            // Search In Cache First
            Dictionary<string, DomainFileEntry> filesList = this.FilesList;
            string cacheSearchKey =
                DomainFileEntry.CreateSearchKey(registrationPath, fileName);

            if (FilesList.ContainsKey(cacheSearchKey))
                return FilesList[cacheSearchKey];
            // !---

            long index = -1, length = -1, compressedLength = -1;
            string localRegistrationPath = null, localFileName = null;

            Stream fileStream = null;
            BinaryReader fileReader = null;
            bool isFound = false;
            try
            {
                fileStream = new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

                    if (string.Compare(registrationPath, localRegistrationPath, true) == 0 &&
                        string.Compare(fileName, localFileName, true) == 0)
                    {
                        isFound = true;

                        break;
                    }
                } while (readC != movedIndex);
            }
            catch (System.Exception)
            {
                isFound = false;
            }
            finally
            {
                if (fileReader != null)
                {
                    fileReader.Close();
                    GC.SuppressFinalize(fileReader);
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

            return new DomainFileEntry(index, localRegistrationPath, localFileName, length, compressedLength);
        }

        public RequestResults ReadFile(long index, long length, ref Stream outputStream)
        {
            RequestResults rRequestResult = RequestResults.None;

            if (index == -1)
                throw new IndexOutOfRangeException();
            if (length < 1)
                throw new ArgumentOutOfRangeException();
            if (outputStream == null)
                throw new NullReferenceException();

            // Search in Cache First
            string searchKey = string.Format("{0}$i:{1}.l:{2}", this._DomainFileLocation, index, length);

            lock (DomainDecompiler._DomainFileEntryBytesCache.SyncRoot)
            {
                if (DomainDecompiler._DomainFileEntryBytesCache.ContainsKey(searchKey))
                {
                    byte[] rbuffer = (byte[])DomainDecompiler._DomainFileEntryBytesCache[searchKey];

                    outputStream.Write(rbuffer, 0, rbuffer.Length);

                    rRequestResult = RequestResults.Authenticated;

                    outputStream.Seek(0, SeekOrigin.Begin);

                    return rRequestResult;
                }
            }
            // !---

            Stream domainFileStream = null;
            Stream gzipHelperStream = null;
            GZipStream gzipCStream = null;

            byte[] buffer = new byte[length];
            try
            {
                domainFileStream = new FileStream(this._DomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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
                int bC = 0, tB = 0;

                do
                {
                    bC = gzipCStream.Read(rbuffer, 0, rbuffer.Length);
                    tB += bC;

                    if (bC > 0)
                        outputStream.Write(rbuffer, 0, bC);
                } while (bC > 0);

                rRequestResult = RequestResults.Authenticated;

                // Cache What You Read
                byte[] cacheBytes = new byte[tB];

                outputStream.Seek(0, SeekOrigin.Begin);
                outputStream.Read(cacheBytes, 0, cacheBytes.Length);

                lock (DomainDecompiler._DomainFileEntryBytesCache.SyncRoot)
                {
                    try
                    {
                        if (DomainDecompiler._DomainFileEntryBytesCache.ContainsKey(searchKey))
                            DomainDecompiler._DomainFileEntryBytesCache.Remove(searchKey);

                        DomainDecompiler._DomainFileEntryBytesCache.Add(searchKey, cacheBytes);
                    }
                    catch (System.Exception)
                    {
                        // Just Handle Exceptions
                        // If an error occur while caching, let it not to be cached.
                    }
                }
                // !---
            }
            catch (FileNotFoundException)
            {
                rRequestResult = RequestResults.ContentNotExists;
            }
            catch (System.Exception)
            {
                rRequestResult = RequestResults.PasswordError;
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

            outputStream.Seek(0, SeekOrigin.Begin);

            return rRequestResult;
        }

        public void ClearCache()
        {
            lock (DomainDecompiler._DomainFileEntryBytesCache.SyncRoot)
            {
                Array keys =
                    Array.CreateInstance(typeof(object), DomainDecompiler._DomainFileEntryBytesCache.Keys.Count);
                DomainDecompiler._DomainFileEntryBytesCache.Keys.CopyTo(keys, 0);

                foreach (object key in keys)
                {
                    string key_s = (string)key;

                    if (key_s.IndexOf(string.Format("{0}$", this._DomainFileLocation)) == 0)
                        DomainDecompiler._DomainFileEntryBytesCache.Remove(key_s);
                }
            }

            Dictionary<string, DomainFileEntry> dummy1;
            DomainDecompiler._DomainFileEntryListCache.TryRemove(this._DomainFileLocation, out dummy1);

            DateTime dummy2;
            DomainDecompiler._DomainFileEntryLastModifiedDate.TryRemove(this._DomainFileLocation, out dummy2);
        }
    }
}
