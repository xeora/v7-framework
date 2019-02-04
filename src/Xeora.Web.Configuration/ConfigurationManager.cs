using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Configuration
{
    public class ConfigurationManager
    {
        private readonly string _ConfigurationPath;
        private readonly string _ConfigurationFile;

        private ConfigurationManager(string configurationPath, string configurationFile)
        {
            this._ConfigurationPath = configurationPath;
            this._ConfigurationFile = configurationFile;
            if (string.IsNullOrEmpty(this._ConfigurationFile))
                this._ConfigurationFile = "xeora.settings.json";

            this.Load();
        }

        private void Load()
        {
            string confFile = Path.Combine(this._ConfigurationPath, this._ConfigurationFile);

            StreamReader sR = null;
            Newtonsoft.Json.JsonReader jsonReader = null;
            try
            {
                sR = new StreamReader(confFile, Encoding.UTF8);
                jsonReader = new Newtonsoft.Json.JsonTextReader(sR);

                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                this.Configuration = jsonSerializer.Deserialize<Xeora>(jsonReader);
            }
            catch (Exception ex)
            {
                throw new ConfigurationWrongException(ex);
            }
            finally
            {
                if (jsonReader != null)
                    jsonReader.Close();

                if (sR != null)
                    sR.Close();
            }
        }

        public Xeora Configuration { get; private set; }

        public static void Initialize(string configurationPath, string configurationFile) =>
            ConfigurationManager._Current = new ConfigurationManager(configurationPath, configurationFile);

        private static ConfigurationManager _Current;
        public static ConfigurationManager Current
        {
            get
            {
                if (ConfigurationManager._Current == null)
                    throw new ConfigurationManagerNotReadyException();

                return ConfigurationManager._Current;
            }
        }
    }
}
