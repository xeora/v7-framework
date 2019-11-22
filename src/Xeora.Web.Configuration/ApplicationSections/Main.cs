using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Xeora.Web.Configuration.ApplicationSections
{
    public class Main : Basics.Configuration.IMain
    {
        public Main()
        {
            this._VirtualRoot = "/";
            this.Debugging = false;
            this.Compression = true;
            this.PrintAnalysis = false;
            this.LogHttpExceptions = true;
            this.UseHtml5Header = false;
            this.Bandwidth = 0;
        }

        [JsonProperty(PropertyName = "defaultDomain", Required = Required.Always)]
        public string[] DefaultDomain { get; private set; }

        [JsonProperty(PropertyName = "physicalRoot", Required = Required.Always)]
        public string PhysicalRoot { get; private set; }

        [DefaultValue("/")]
        [JsonProperty(PropertyName = "virtualRoot", DefaultValueHandling = DefaultValueHandling.Populate)]
        private string _VirtualRoot { get; set; }

        private bool _IsVirtualRootFixed;
        public string VirtualRoot
        {
            get
            {
                if (this._IsVirtualRootFixed) return this._VirtualRoot;
                
                string virtualRoot = this._VirtualRoot;

                if (string.IsNullOrEmpty(virtualRoot))
                    virtualRoot = "/";

                virtualRoot = virtualRoot.Replace('\\', '/');

                if (virtualRoot.IndexOf('/') != 0)
                    virtualRoot = $"/{virtualRoot}";

                if (virtualRoot[virtualRoot.Length - 1] != '/')
                    virtualRoot = $"{virtualRoot}/";

                this._VirtualRoot = virtualRoot;
                this._IsVirtualRootFixed = true;

                return this._VirtualRoot;
            }
        }

        [JsonProperty(PropertyName = "applicationRoot")]
        private string _ApplicationRoot { get; set; }

        private Basics.Configuration.IApplicationRootFormat _ApplicationRootFormat;
        public Basics.Configuration.IApplicationRootFormat ApplicationRoot
        {
            get
            {
                if (this._ApplicationRootFormat != null) return this._ApplicationRootFormat;
                
                this._ApplicationRootFormat = new ApplicationRootFormat();

                if (string.IsNullOrEmpty(this._ApplicationRoot))
                    this._ApplicationRoot = $".{Path.DirectorySeparatorChar}";

                if (this._ApplicationRoot.IndexOf(Path.DirectorySeparatorChar) == 0)
                    this._ApplicationRoot = $".{this._ApplicationRoot}";

                if (this._ApplicationRoot.IndexOf($".{Path.DirectorySeparatorChar}", StringComparison.Ordinal) != 0)
                    this._ApplicationRoot = $".{Path.DirectorySeparatorChar}{this._ApplicationRoot}";

                if (this._ApplicationRoot[^1] != Path.DirectorySeparatorChar)
                    this._ApplicationRoot = $"{this._ApplicationRoot}{Path.DirectorySeparatorChar}";

                ((ApplicationRootFormat)this._ApplicationRootFormat).FileSystemImplementation = this._ApplicationRoot;
                ((ApplicationRootFormat)this._ApplicationRootFormat).BrowserImplementation =
                    $"{this.VirtualRoot}{this._ApplicationRoot.Substring(2).Replace('\\', '/')}";

                return this._ApplicationRootFormat;
            }
        }

        private Basics.Configuration.IWorkingPathFormat _WorkingPathFormat;
        public Basics.Configuration.IWorkingPathFormat WorkingPath
        {
            get
            {
                if (this._WorkingPathFormat != null) return this._WorkingPathFormat;
                
                this._WorkingPathFormat = new WorkingPathFormat();

                string wPath =
                    Path.GetFullPath(Path.Combine(this.PhysicalRoot, this.ApplicationRoot.FileSystemImplementation));
                ((WorkingPathFormat)this._WorkingPathFormat).WorkingPath = wPath;

                foreach (Match match in Regex.Matches(wPath, "\\W"))
                {
                    if (!match.Success) continue;
                        
                    wPath = wPath.Remove(match.Index, match.Length);
                    wPath = wPath.Insert(match.Index, "_");
                }
                ((WorkingPathFormat)this._WorkingPathFormat).WorkingPathId = wPath;

                return this._WorkingPathFormat;
            }
        }

        private string _TemporaryRoot;
        public string TemporaryRoot
        {
            get
            {
                if (this._TemporaryRoot != null) return this._TemporaryRoot;
                
                this._TemporaryRoot = Path.GetTempPath();

                if (string.IsNullOrEmpty(this._TemporaryRoot))
                    this._TemporaryRoot = Path.Combine(this.PhysicalRoot, "tmp");

                if (!Directory.Exists(this._TemporaryRoot))
                    Directory.CreateDirectory(this._TemporaryRoot);

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
        [JsonProperty(PropertyName = "printAnalysis", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool PrintAnalysis { get; private set; }
        
        [DefaultValue(1000)]
        [JsonProperty(PropertyName = "analysisThreshold", DefaultValueHandling = DefaultValueHandling.Populate)]
        public double AnalysisThreshold { get; private set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "logHTTPExceptions", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool LogHttpExceptions { get; private set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "useHTML5Header", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseHtml5Header { get; private set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "bandwidth", DefaultValueHandling = DefaultValueHandling.Populate)]
        public long Bandwidth { get; private set; }
    }
}
