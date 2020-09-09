using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Configuration
{
    public class Manager
    {
        private readonly string _ConfigurationPath;
        private readonly string _ConfigurationFile;

        private Manager(string configurationPath, string configurationFile)
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
                throw new Exceptions.ConfigurationWrongException(ex);
            }
            finally
            {
                jsonReader?.Close();
                sR?.Close();
            }
        }

        public Xeora Configuration { get; private set; }

        public static void Initialize(string configurationPath, string configurationFile) =>
            Manager._current = new Manager(configurationPath, configurationFile);

        private static Manager _current;
        public static Manager Current
        {
            get
            {
                if (Manager._current == null)
                    throw new Exceptions.ConfigurationManagerNotReadyException();

                return Manager._current;
            }
        }
    }
}
