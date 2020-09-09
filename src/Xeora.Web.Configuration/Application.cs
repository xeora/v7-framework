using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Xeora.Web.Configuration
{
    public class Application : Basics.Configuration.IApplication
    {
        public Application()
        {
            this.Main = new ApplicationSections.Main();
            this.RequestTagFilter = new ApplicationSections.RequestTagFilter();
            this.CustomMimes = new ApplicationSections.MimeItem[] { };
            this._BannedFiles = new string[] { };
        }

        [JsonProperty(PropertyName = "main", Required = Required.Always)]
        public Basics.Configuration.IMain Main { get; private set; }

        [JsonProperty(PropertyName = "requestTagFilter")]
        public Basics.Configuration.IRequestTagFilter RequestTagFilter { get; private set; }

        [JsonProperty(PropertyName = "customMimes")]
        public IEnumerable<Basics.Configuration.IMimeItem> CustomMimes { get; private set; }

        [JsonProperty(PropertyName = "bannedFiles")]
        private string[] _BannedFiles { get; set; }

        private bool _isBannedFilesFixed;
        public string[] BannedFiles
        {
            get
            {
                if (this._isBannedFilesFixed) return this._BannedFiles;
                
                this._isBannedFilesFixed = true;

                // \\ < regex definition
                string forbiddenDomain = $"\\{Path.DirectorySeparatorChar}Domains";
                if (Array.IndexOf(this._BannedFiles, forbiddenDomain) == -1)
                {
                    string[] fixedBannedFiles = new string[this._BannedFiles.Length + 1];
                    Array.Copy(this._BannedFiles, fixedBannedFiles, this.BannedFiles.Length);

                    fixedBannedFiles[^1] = forbiddenDomain;

                    this._BannedFiles = fixedBannedFiles;
                }

                // \\ < regex definition
                string forbiddenXeoraSettingsJson =
                    $"\\{Path.DirectorySeparatorChar}xeora\\.settings\\.json";
                if (Array.IndexOf(this._BannedFiles, forbiddenXeoraSettingsJson) == -1)
                {
                    string[] fixedBannedFiles = new string[this._BannedFiles.Length + 1];
                    Array.Copy(this._BannedFiles, fixedBannedFiles, this.BannedFiles.Length);

                    fixedBannedFiles[^1] = forbiddenXeoraSettingsJson;

                    this._BannedFiles = fixedBannedFiles;
                }

                return this._BannedFiles;
            }
        }
    }
}
