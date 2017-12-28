using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Xeora.Web.Deployment
{
    public abstract class DeploymentBase
    {
        private string _LanguageID;

        public abstract void Dispose();
        public abstract Basics.ISettings Settings { get; }
        public abstract Basics.ILanguage Language { get; }
        public abstract Basics.IxService xService { get; }
        public abstract Basics.DomainInfo.DomainInfoCollection Children { get; }
        public abstract void ProvideFileStream(string requestedFilePath, out Stream outputStream);

        public DeploymentBase(string[] domainIDAccessTree, string languageID)
        {
            this.DomainIDAccessTree = domainIDAccessTree;
            this._LanguageID = languageID;

            if (this.DomainIDAccessTree == null || 
                this.DomainIDAccessTree.Length == 0)
                throw new Exception.DeploymentException(Global.SystemMessages.IDMUSTBESET);

            this.WorkingRoot = 
                Path.GetFullPath(
                    Path.Combine(
                        Basics.Configurations.Xeora.Application.Main.PhysicalRoot, 
                        Basics.Configurations.Xeora.Application.Main.ApplicationRoot.FileSystemImplementation, 
                        "Domains", 
                        this.CreateDomainAccessPathString()
                    )
                );

            if (!Directory.Exists(this.WorkingRoot))
                throw new Exception.DomainNotExistsException(
                    Global.SystemMessages.PATH_NOTEXISTS, 
                    new DirectoryNotFoundException(string.Format("WorkingPath: {0}", this.WorkingRoot))
                );

            string releaseTestPath = 
                Path.Combine(this.WorkingRoot, "Content.xeora");

            if (File.Exists(releaseTestPath))
            {
                this.DeploymentType = Basics.DomainInfo.DeploymentTypes.Release;

                this.Decompiler = new XeoraDomainDecompiler(this.WorkingRoot);
            }
            else
                this.DeploymentType = Basics.DomainInfo.DeploymentTypes.Development;
        }

        public Basics.DomainInfo.DeploymentTypes DeploymentType { get; private set; }
        public string[] DomainIDAccessTree { get; private set; }

        public string LanguageID
        {
            get { return this._LanguageID; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    this._LanguageID = value;
                else
                    throw new Exception.DeploymentException("LanguageID must not be null", new NoNullAllowedException());
            }
        }

        protected XeoraDomainDecompiler Decompiler { get; private set; }
        protected string WorkingRoot { get; private set; }

        protected string ExecutablesPath => Path.Combine(this.WorkingRoot, "Executables");

        protected string TemplatesRegistration
        {
            get
            {
                if (this.DeploymentType == Basics.DomainInfo.DeploymentTypes.Release)
                    return "\\Templates\\";
                
                return Path.Combine(this.WorkingRoot, "Templates");
            }
        }

        protected string DomainContentsRegistration(string languageID = null)
        {
            if (string.IsNullOrEmpty(languageID))
                languageID = this._LanguageID;

            if (this.DeploymentType == Basics.DomainInfo.DeploymentTypes.Release)
                return string.Format("\\Contents\\{0}\\", languageID);
                
            return Path.Combine(this.WorkingRoot, "Contents", languageID);
        }

        protected string LanguagesRegistration
        {
            get
            {
                if (this.DeploymentType == Basics.DomainInfo.DeploymentTypes.Release)
                    return "\\Languages\\";
                
                return Path.Combine(this.WorkingRoot, "Languages");
            }
        }

        protected string ChildrenRootPath => Path.Combine(this.WorkingRoot, "Addons");

        private string CreateDomainAccessPathString()
        {
            string rDomainAccessPath = this.DomainIDAccessTree[0];

            for (int iC = 1; iC < this.DomainIDAccessTree.Length; iC++)
                rDomainAccessPath = Path.Combine(rDomainAccessPath, "Addons", this.DomainIDAccessTree[iC]);

            return rDomainAccessPath;
        }

        private Encoding DetectEncoding(ref Stream inStream)
        {
            inStream.Seek(0, SeekOrigin.Begin);

            int bC = 0;
            byte[] buffer = new byte[4];

            bC = inStream.Read(buffer, 0, buffer.Length);

            if (bC > 0)
            {
                if (bC >= 2 && buffer[0] == 254 && buffer[1] == 255)
                {
                    inStream.Seek(2, SeekOrigin.Begin);

                    return new UnicodeEncoding(true, true);
                }
                else if (bC >= 2 && buffer[0] == 255 && buffer[1] == 254)
                {
                    if (bC == 4 && buffer[2] == 0 && buffer[3] == 0)
                    {
                        inStream.Seek(4, SeekOrigin.Begin);

                        return new UTF32Encoding(false, true);
                    }

                    inStream.Seek(2, SeekOrigin.Begin);

                    return new UnicodeEncoding(false, true);
                }
                else if (bC >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
                {
                    inStream.Seek(3, SeekOrigin.Begin);

                    return new UTF8Encoding();
                }
                else if (bC == 4 && buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 254 && buffer[3] == 255)
                {
                    inStream.Seek(4, SeekOrigin.Begin);

                    return new UTF32Encoding(true, true);
                }

                inStream.Seek(0, SeekOrigin.Begin);
            }

            return Encoding.UTF8;
        }

        public bool CheckTemplateExists(string serviceFullPath)
        {
            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    return File.Exists(
                        Path.Combine(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath)));
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo XeoraFileInfo = 
                        this.Decompiler.GetFileInfo(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    return XeoraFileInfo.Index > -1;
            }

            return false;
        }

        public virtual string ProvideTemplateContent(string serviceFullPath)
        {
            string rTemplateContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    string templateFile = 
                        Path.Combine(this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder templateContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                templateContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rTemplateContent = templateContent.ToString();
                    }
                    catch (System.Exception)
                    {
                        rTemplateContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo = 
                        this.Decompiler.GetFileInfo(
                            this.TemplatesRegistration, string.Format("{0}.xchtml", serviceFullPath));

                    if (xeoraFileInfo.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        XeoraDomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case XeoraDomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rTemplateContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case XeoraDomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rTemplateContent;
        }

        public string ProvideLanguageContent(string languageID)
        {
            string rLanguageContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    string languageFile = 
                        Path.Combine(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder languageContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(languageFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                languageContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        this._LanguageID = languageID;
                        rLanguageContent = languageContent.ToString();
                    }
                    catch (FileNotFoundException)
                    {
                        if (string.Compare(languageID, this.Settings.Configurations.DefaultLanguage) != 0)
                            rLanguageContent = this.ProvideLanguageContent(this.Settings.Configurations.DefaultLanguage);
                    }
                    catch (System.Exception)
                    {
                        rLanguageContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo = 
                        this.Decompiler.GetFileInfo(this.LanguagesRegistration, string.Format("{0}.xml", languageID));

                    if (xeoraFileInfo.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        XeoraDomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case XeoraDomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rLanguageContent = sR.ReadToEnd();

                                this._LanguageID = languageID;

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case XeoraDomainDecompiler.RequestResults.ContentNotExists:
                                if (string.Compare(languageID, this.Settings.Configurations.DefaultLanguage) != 0)
                                    rLanguageContent = this.ProvideLanguageContent(this.Settings.Configurations.DefaultLanguage);

                                break;
                            case XeoraDomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rLanguageContent;
        }
        
        public string ProvideConfigurationContent()
        {
            string rConfigurationContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    string configurationFile = Path.Combine(this.TemplatesRegistration, "Configuration.xml");

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder configurationContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(configurationFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                configurationContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rConfigurationContent = configurationContent.ToString();
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, ex);
                    }
                    catch (System.Exception)
                    {
                        rConfigurationContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo = 
                        this.Decompiler.GetFileInfo(this.TemplatesRegistration, "Configuration.xml");

                    if (xeoraFileInfo.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        XeoraDomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case XeoraDomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rConfigurationContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case XeoraDomainDecompiler.RequestResults.ContentNotExists:
                                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONFIGURATIONNOTFOUND, new FileNotFoundException());
                            case XeoraDomainDecompiler.RequestResults.PasswordError:
                                throw new Exception.DeploymentException(Global.SystemMessages.PASSWORD_WRONG, new SecurityException());
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rConfigurationContent;
        }

        public string ProvideControlsContent()
        {
            string rControlMapContent = string.Empty;

            switch (this.DeploymentType)
            {
                case Basics.DomainInfo.DeploymentTypes.Development:
                    string controlsXMLFile = Path.Combine(this.TemplatesRegistration, "Controls.xml");

                    byte[] buffer = new byte[102400];
                    int rB = 0;

                    Encoding encoding;
                    StringBuilder controlMapContent = new StringBuilder();

                    Stream fS = null;
                    try
                    {
                        fS = new FileStream(controlsXMLFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        encoding = this.DetectEncoding(ref fS);

                        do
                        {
                            rB = fS.Read(buffer, 0, buffer.Length);

                            if (rB > 0)
                                controlMapContent.Append(encoding.GetString(buffer, 0, rB));
                        } while (rB != 0);

                        rControlMapContent = controlMapContent.ToString();
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, ex);
                    }
                    catch (System.Exception)
                    {
                        rControlMapContent = string.Empty;
                    }
                    finally
                    {
                        if (fS != null)
                        {
                            fS.Close();
                            GC.SuppressFinalize(fS);
                        }
                    }

                    break;
                case Basics.DomainInfo.DeploymentTypes.Release:
                    XeoraDomainDecompiler.XeoraFileInfo xeoraFileInfo = 
                        this.Decompiler.GetFileInfo(this.TemplatesRegistration, "Controls.xml");

                    if (xeoraFileInfo.Index > -1)
                    {
                        Stream contentStream = new MemoryStream();

                        XeoraDomainDecompiler.RequestResults requestResult = 
                            this.Decompiler.ReadFile(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref contentStream);

                        switch (requestResult)
                        {
                            case XeoraDomainDecompiler.RequestResults.Authenticated:
                                StreamReader sR = new StreamReader(contentStream);

                                rControlMapContent = sR.ReadToEnd();

                                sR.Close();
                                GC.SuppressFinalize(sR);

                                break;
                            case XeoraDomainDecompiler.RequestResults.ContentNotExists:
                                throw new Exception.DeploymentException(Global.SystemMessages.ESSENTIAL_CONTROLSXMLNOTFOUND, new FileNotFoundException());
                            case XeoraDomainDecompiler.RequestResults.PasswordError:
                                throw new System.Exception(Global.SystemMessages.PASSWORD_WRONG);
                        }

                        if (contentStream != null)
                        {
                            contentStream.Close();
                            GC.SuppressFinalize(contentStream);
                        }
                    }

                    break;
            }

            return rControlMapContent;
        }

        #region " Xeora Domain Decompiler "
        protected class XeoraDomainDecompiler
        {
            public class XeoraFileInfo
            {
                internal XeoraFileInfo(long index, string registrationPath, string fileName, long length, long compressedLength)
                {
                    this.Index = Index;
                    this.RegistrationPath = RegistrationPath;
                    this.FileName = FileName;
                    this.Length = Length;
                    this.CompressedLength = CompressedLength;
                }

                public long Index { get; private set; }
                public string RegistrationPath { get; private set; }
                public string FileName { get; private set; }
                public string SearchKey => XeoraFileInfo.CreateSearchKey(this.RegistrationPath, this.FileName);
                public long Length { get; private set; }
                public long CompressedLength { get; private set; }

                public static string CreateSearchKey(string registrationPath, string fileName) =>
                    string.Format("{0}${1}", registrationPath, fileName);
            }

            private string _XeoraDomainFileLocation;
            private byte[] _PasswordHash = null;

            private static ConcurrentDictionary<string, Dictionary<string, XeoraFileInfo>> _XeoraDomainFilesListCache = 
                new ConcurrentDictionary<string, Dictionary<string, XeoraFileInfo>>();
            private static Hashtable _XeoraDomainFileStreamBytesCache = 
                Hashtable.Synchronized(new Hashtable());
            private static ConcurrentDictionary<string, DateTime> _XeoraDomainFileLastModifiedDate = 
                new ConcurrentDictionary<string, DateTime>();

            public enum RequestResults
            {
                None,
                Authenticated,
                PasswordError,
                ContentNotExists
            }

            public XeoraDomainDecompiler(string xeoraDomainRoot)
            {
                this._XeoraDomainFileLocation = 
                    Path.Combine(xeoraDomainRoot, "Content.xeora");
                string domainPasswordFileLocation = 
                    Path.Combine(xeoraDomainRoot, "Content.secure");

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
                        contentFS = new FileStream(this._XeoraDomainFileLocation, FileMode.Open, FileAccess.Read);

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

                FileInfo fI = new FileInfo(this._XeoraDomainFileLocation);

                if (fI.Exists)
                    XeoraDomainDecompiler._XeoraDomainFileLastModifiedDate.TryAdd(this._XeoraDomainFileLocation, fI.CreationTime);
            }

            public Dictionary<string, XeoraFileInfo> FilesList
            {
                get
                {
                    // Control Template File Changes
                    DateTime cachedFileDate;
                    if (!XeoraDomainDecompiler._XeoraDomainFileLastModifiedDate.TryGetValue(this._XeoraDomainFileLocation, out cachedFileDate))
                        cachedFileDate = DateTime.MinValue;

                    FileInfo fI = new FileInfo(this._XeoraDomainFileLocation);

                    if (fI.Exists && DateTime.Compare(cachedFileDate, fI.CreationTime) != 0)
                        this.ClearCache();
                    // !---

                    Dictionary<string, XeoraFileInfo> rFileList;

                    if (!XeoraDomainDecompiler._XeoraDomainFilesListCache.TryGetValue(this._XeoraDomainFileLocation, out rFileList))
                    {
                        rFileList = new Dictionary<string, XeoraFileInfo>();

                        XeoraFileInfo[] xeoraFileInfoList = this.ReadFileList();

                        foreach (XeoraFileInfo xeoraFileInfo in xeoraFileInfoList)
                            rFileList.Add(xeoraFileInfo.SearchKey, xeoraFileInfo);

                        XeoraDomainDecompiler._XeoraDomainFilesListCache.TryAdd(this._XeoraDomainFileLocation, rFileList);
                    }

                    return rFileList;
                }
            }

            private XeoraFileInfo[] ReadFileList()
            {
                List<XeoraFileInfo> rXeoraFileInfo = new List<XeoraFileInfo>();

                long index = -1, length = -1, compressedLength = -1;
                string localRegistrationPath = null, localFileName = null;

                Stream xeoraFileStream = null;
                BinaryReader xeoraStreamBinaryReader = null;
                try
                {
                    xeoraFileStream = new FileStream(this._XeoraDomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    xeoraStreamBinaryReader = new BinaryReader(xeoraFileStream, Encoding.UTF8);

                    int readC = 0;
                    long indexTotal = 0, movedIndex = xeoraStreamBinaryReader.ReadInt64();

                    do
                    {
                        indexTotal = xeoraStreamBinaryReader.BaseStream.Position;

                        index = xeoraStreamBinaryReader.ReadInt64() + movedIndex + 8;
                        localRegistrationPath = xeoraStreamBinaryReader.ReadString();
                        localFileName = xeoraStreamBinaryReader.ReadString();
                        length = xeoraStreamBinaryReader.ReadInt64();
                        compressedLength = xeoraStreamBinaryReader.ReadInt64();

                        readC += (int)(xeoraStreamBinaryReader.BaseStream.Position - indexTotal);

                        rXeoraFileInfo.Add(
                            new XeoraFileInfo(index, localRegistrationPath, localFileName, length, compressedLength));
                    } while (readC != movedIndex);
                }
                finally
                {
                    if (xeoraStreamBinaryReader != null)
                    {
                        xeoraStreamBinaryReader.Close();
                        GC.SuppressFinalize(xeoraStreamBinaryReader);
                    }
                }

                return rXeoraFileInfo.ToArray();
            }

            public XeoraFileInfo GetFileInfo(string registrationPath, string fileName)
            {
                // Search In Cache First
                Dictionary<string, XeoraFileInfo> filesList = this.FilesList;
                string cacheSearchKey = 
                    XeoraFileInfo.CreateSearchKey(registrationPath, fileName);

                if (FilesList.ContainsKey(cacheSearchKey))
                    return FilesList[cacheSearchKey];
                // !---

                long index = -1, length = -1, compressedLength = -1;
                string localRegistrationPath = null, localFileName = null;

                Stream xeoraFileStream = null;
                BinaryReader xeoraStreamBinaryReader = null;
                bool isFound = false;
                try
                {
                    xeoraFileStream = new FileStream(this._XeoraDomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    xeoraStreamBinaryReader = new BinaryReader(xeoraFileStream, Encoding.UTF8);

                    int readC = 0;
                    long indexTotal = 0, movedIndex = xeoraStreamBinaryReader.ReadInt64();

                    do
                    {
                        indexTotal = xeoraStreamBinaryReader.BaseStream.Position;

                        index = xeoraStreamBinaryReader.ReadInt64() + movedIndex + 8;
                        localRegistrationPath = xeoraStreamBinaryReader.ReadString();
                        localFileName = xeoraStreamBinaryReader.ReadString();
                        length = xeoraStreamBinaryReader.ReadInt64();
                        compressedLength = xeoraStreamBinaryReader.ReadInt64();

                        readC += (int)(xeoraStreamBinaryReader.BaseStream.Position - indexTotal);

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

                return new XeoraFileInfo(index, localRegistrationPath, localFileName, length, compressedLength);
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
                string searchKey = string.Format("{0}$i:{1}.l:{2}", this._XeoraDomainFileLocation, index, length);

                lock (XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.SyncRoot)
                {
                    if (XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.ContainsKey(searchKey))
                    {
                        byte[] rbuffer = (byte[])XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache[searchKey];

                        outputStream.Write(rbuffer, 0, rbuffer.Length);

                        rRequestResult = RequestResults.Authenticated;

                        outputStream.Seek(0, SeekOrigin.Begin);

                        return rRequestResult;
                    }
                }
                // !---

                Stream xeoraFileStream = null;
                Stream gzipHelperStream = null;
                GZipStream gzipCStream = null;

                byte[] buffer = new byte[length];
                try
                {
                    xeoraFileStream = new FileStream(this._XeoraDomainFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    xeoraFileStream.Seek(index, SeekOrigin.Begin);
                    xeoraFileStream.Read(buffer, 0, buffer.Length);

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

                    lock (XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.SyncRoot)
                    {
                        try
                        {
                            if (XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.ContainsKey(searchKey))
                                XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.Remove(searchKey);

                            XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.Add(searchKey, cacheBytes);
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
                    if (xeoraFileStream != null)
                    {
                        xeoraFileStream.Close();
                        GC.SuppressFinalize(xeoraFileStream);
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
                lock (XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.SyncRoot)
                {
                    Array keys = 
                        Array.CreateInstance(typeof(object), XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.Keys.Count);
                    XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.Keys.CopyTo(keys, 0);

                    foreach (object key in keys)
                    {
                        string key_s = (string)key;

                        if (key_s.IndexOf(string.Format("{0}$", this._XeoraDomainFileLocation)) == 0)
                            XeoraDomainDecompiler._XeoraDomainFileStreamBytesCache.Remove(key_s);
                    }
                }

                Dictionary<string, XeoraFileInfo> dummy1;
                XeoraDomainDecompiler._XeoraDomainFilesListCache.TryRemove(this._XeoraDomainFileLocation, out dummy1);

                DateTime dummy2;
                XeoraDomainDecompiler._XeoraDomainFileLastModifiedDate.TryRemove(this._XeoraDomainFileLocation, out dummy2);
            }
        }
        #endregion
    }
}
