using System.Reflection;

namespace Xeora.Web.Basics
{
    public class Configurations
    {
        private static Configuration.IXeora _xeora = null;
        /// <summary>
        /// Gets the Xeora framework configurations
        /// </summary>
        /// <value>Xeora framwork configuration instance</value>
        public static Configuration.IXeora Xeora
        {
            get
            {
                if (Configurations._xeora == null)
                    Configurations._xeora =
                        (Configuration.IXeora)TypeCache.Instance.RemoteInvoke.InvokeMember("XeoraSettings", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);

                return Configurations._xeora;
            }
        }
    }
}
