using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Xeora.Web.Tools.Serialization
{
    public static class Binary
    {
        public static byte[] Serialize(object value)
        {
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream();
                
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(forStream, value);

                return ((MemoryStream)forStream).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                forStream?.Close();
            }
        }

        public static T DeSerialize<T>(byte[] value)
        {
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream(value);

                BinaryFormatter binFormatter = new BinaryFormatter();

                return (T)Convert.ChangeType(binFormatter.Deserialize(forStream), typeof(T));
            }
            catch (Exception)
            {
                return default;
            }
            finally
            {
                forStream?.Close();
            }
        }
    }
}
