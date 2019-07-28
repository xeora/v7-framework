namespace Xeora.Web.Exceptions
{
    public class ConfigurationWrongException : System.Exception
    {
        public ConfigurationWrongException() : base()
        { }

        public ConfigurationWrongException(System.Exception exception) : base("Configuration Loading Error", exception)
        { }
    }
}
