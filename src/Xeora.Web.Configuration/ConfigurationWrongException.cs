namespace Xeora.Web.Configuration
{
    public class ConfigurationWrongException : System.Exception
    {
        public ConfigurationWrongException() : base()
        { }

        public ConfigurationWrongException(System.Exception exception) : base("Configuration Loading Error", exception)
        { }
    }
}
