namespace Xeora.Web.Basics
{
    public static class Configurations
    {
        private static Configuration.IXeora _xeora;
        /// <summary>
        /// Gets the Xeora framework configurations
        /// </summary>
        /// <value>Xeora framework configuration instance</value>
        public static Configuration.IXeora Xeora =>
            Configurations._xeora ?? (Configurations._xeora = Helpers.Negotiator.XeoraSettings);
    }
}
