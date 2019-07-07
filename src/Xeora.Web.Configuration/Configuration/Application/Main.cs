using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Xeora.Web.Configuration
{
    public class Main : Basics.Configuration.IMain
    {
        public Main()
        {
            this._VirtualRoot = "/";
            this.Debugging = false;
            this.Compression = true;
            this.PrintAnalytics = false;
            this.LogHTTPExceptions = true;
            this.UseHTML5Header = false;
            this.Bandwidth = 0;
        }

        [JsonProperty(PropertyName = "defaultDomain", Required = Required.Always)]
        public string[] DefaultDomain { get; private set; }

        [JsonProperty(PropertyName = "physicalRoot", Required = Required.Always)]
        public string PhysicalRoot { get; private set; }

        [DefaultValue("/")]
        [JsonProperty(PropertyName = "virtualRoot", DefaultValueHandling = DefaultValueHandling.Populate)]
        private string _VirtualRoot { get; set; }

        private bool _IsVirtualRootFixed = false;
        public string VirtualRoot
        {
            get
            {
                if (!this._IsVirtualRootFixed)
                {
                    string virtualRoot = this._VirtualRoot;

                    if (string.IsNullOrEmpty(virtualRoot))
                        virtualRoot = "/";

                    virtualRoot = virtualRoot.Replace('\\', '/');

                    if (virtualRoot.IndexOf('/') != 0)
                        virtualRoot = string.Format("/{0}", virtualRoot);

                    if (virtualRoot[virtualRoot.Length - 1] != '/')
                        virtualRoot = string.Format("{0}/", virtualRoot);

                    this._VirtualRoot = virtualRoot;
                    this._IsVirtualRootFixed = true;
                }

                return this._VirtualRoot;
            }
        }

        [JsonProperty(PropertyName = "applicationRoot")]
        private string _ApplicationRoot { get; set; }

        private Basics.Configuration.IApplicationRootFormat _ApplicationRootFormat = null;
        public Basics.Configuration.IApplicationRootFormat ApplicationRoot
        {
            get
            {
                if (this._ApplicationRootFormat == null)
                {
                    this._ApplicationRootFormat = new ApplicationRootFormat();

                    if (string.IsNullOrEmpty(this._ApplicationRoot))
                        this._ApplicationRoot = string.Format(".{0}", Path.DirectorySeparatorChar);

                    if (this._ApplicationRoot.IndexOf(Path.DirectorySeparatorChar) == 0)
                        this._ApplicationRoot = string.Format(".{0}", this._ApplicationRoot);

                    if (this._ApplicationRoot.IndexOf(string.Format(".{0}", Path.DirectorySeparatorChar)) != 0)
                        this._ApplicationRoot = string.Format(".{0}{1}", Path.DirectorySeparatorChar, this._ApplicationRoot);

                    if (this._ApplicationRoot[this._ApplicationRoot.Length - 1] != Path.DirectorySeparatorChar)
                        this._ApplicationRoot = string.Format("{0}{1}", this._ApplicationRoot, Path.DirectorySeparatorChar);

                    ((ApplicationRootFormat)this._ApplicationRootFormat).FileSystemImplementation = this._ApplicationRoot;
                    ((ApplicationRootFormat)this._ApplicationRootFormat).BrowserImplementation =
                        string.Format("{0}{1}", this.VirtualRoot, this._ApplicationRoot.Substring(2).Replace('\\', '/'));
                }

                return this._ApplicationRootFormat;
            }
        }

        private Basics.Configuration.IWorkingPathFormat _WorkingPathFormat = null;
        public Basics.Configuration.IWorkingPathFormat WorkingPath
        {
            get
            {
                if (this._WorkingPathFormat == null)
                {
                    this._WorkingPathFormat = new WorkingPathFormat();

                    string wPath =
                        Path.GetFullPath(Path.Combine(this.PhysicalRoot, this.ApplicationRoot.FileSystemImplementation));
                    ((WorkingPathFormat)this._WorkingPathFormat).WorkingPath = wPath;

                    foreach (Match match in Regex.Matches(wPath, "\\W"))
                    {
                        if (match.Success)
                        {
                            wPath = wPath.Remove(match.Index, match.Length);
                            wPath = wPath.Insert(match.Index, "_");
                        }
                    }
                    ((WorkingPathFormat)this._WorkingPathFormat).WorkingPathId = wPath;
                }

                return this._WorkingPathFormat;
            }
        }

        private string _TemporaryRoot = null;
        public string TemporaryRoot
        {
            get
            {
                if (this._TemporaryRoot == null)
                {
                    this._TemporaryRoot = Path.GetTempPath();

                    if (string.IsNullOrEmpty(this._TemporaryRoot))
                        this._TemporaryRoot = Path.Combine(this.PhysicalRoot, "tmp");

                    if (!Directory.Exists(this._TemporaryRoot))
                        Directory.CreateDirectory(this._TemporaryRoot);
                }

                return this._TemporaryRoot;
            }
        }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "debugging", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Debugging { get; private set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "compression", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Compression { get; private set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "printAnalytics", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool PrintAnalytics { get; private set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "logHTTPExceptions", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool LogHTTPExceptions { get; private set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "useHTML5Header", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseHTML5Header { get; private set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "bandwidth", DefaultValueHandling = DefaultValueHandling.Populate)]
        public long Bandwidth { get; private set; }

        [JsonProperty(PropertyName = "loggingPath")]
        private string _LoggingPath { get; set; }

        public string LoggingPath
        {
            get
            {
                if (string.IsNullOrEmpty(this._LoggingPath))
                    return Path.Combine(this.PhysicalRoot, "XeoraLogs");

                return this._LoggingPath;
            }
        }
    }
}
