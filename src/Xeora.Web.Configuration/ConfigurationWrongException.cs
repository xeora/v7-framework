using System;

namespace Xeora.Web.Configuration
{
    public class ConfigurationWrongException : Exception
    {
        public ConfigurationWrongException() : base()
        { }

        public ConfigurationWrongException(Exception exception) : base("Configuration Loading Error", exception)
        { }
    }
}
