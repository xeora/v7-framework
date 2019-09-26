namespace Xeora.Web.Exceptions
{
    public class ConfigurationWrongException : System.Exception
    {
        public ConfigurationWrongException(System.Exception exception) : base("Configuration Loading Error", exception)
        { }
    }
}
