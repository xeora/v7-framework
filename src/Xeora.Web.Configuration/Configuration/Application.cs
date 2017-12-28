using Newtonsoft.Json;
using System;
using System.IO;

namespace Xeora.Web.Configuration
{
    public class Application : Basics.Configuration.IApplication
    {
        public Application()
        {
            this.Main = new Main();
            this.RequestTagFilter = new RequestTagFilter();
            this.ServicePort = new ServicePort();
            this.CustomMimes = new MimeItem[] { };
            this._BannedFiles = new string[] { };
        }

        [JsonProperty(PropertyName = "main", Required = Required.Always)]
        public Basics.Configuration.IMain Main { get; private set; }

        [JsonProperty(PropertyName = "requestTagFilter")]
        public Basics.Configuration.IRequestTagFilter RequestTagFilter { get; private set; }

        [JsonProperty(PropertyName = "servicePort")]
        public Basics.Configuration.IServicePort ServicePort { get; private set; }

        [JsonProperty(PropertyName = "customMimes")]
        public Basics.Configuration.IMimeItem[] CustomMimes { get; private set; }

        [JsonProperty(PropertyName = "bannedFiles")]
        private string[] _BannedFiles { get; set; }

        private bool _isBannedFilesFixed = false;
        public string[] BannedFiles
        {
            get
            {
                if (!this._isBannedFilesFixed)
                {
                    this._isBannedFilesFixed = true;

                    // \\ < regex definition
                    string forbiddenDomain = string.Format("\\{0}Domains", Path.DirectorySeparatorChar);

                    if (Array.IndexOf(this._BannedFiles, forbiddenDomain) == -1)
                    {
                        string[] fixedBannedFiles = new string[this._BannedFiles.Length + 1];
                        Array.Copy(this._BannedFiles, fixedBannedFiles, this.BannedFiles.Length);

                        fixedBannedFiles[fixedBannedFiles.Length - 1] = forbiddenDomain;

                        this._BannedFiles = fixedBannedFiles;
                    }
                }

                return this._BannedFiles;
            }
        }
    }
}
