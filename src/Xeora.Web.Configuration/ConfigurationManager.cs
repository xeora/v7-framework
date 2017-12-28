using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Configuration
{
    public class ConfigurationManager
    {
        private string _ConfigurationPath;
        private Xeora _XeoraConfiguration;

        private ConfigurationManager(string configurationPath)
        {
            this._ConfigurationPath = configurationPath;

            this.Load();
        }

        private void Load()
        {
            string confFile = Path.Combine(this._ConfigurationPath, "xeora.settings.json");

            StreamReader sR = null;
            Newtonsoft.Json.JsonReader jsonReader = null;
            try
            {
                sR = new StreamReader(confFile, Encoding.UTF8);
                jsonReader = new Newtonsoft.Json.JsonTextReader(sR);

                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                this._XeoraConfiguration = jsonSerializer.Deserialize<Xeora>(jsonReader);
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

        public Xeora Configuration => this._XeoraConfiguration;

        public static void Initialize(string configurationPath) =>
            ConfigurationManager._Current = new ConfigurationManager(configurationPath);

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
