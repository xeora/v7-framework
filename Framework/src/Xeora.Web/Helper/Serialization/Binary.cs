using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Xeora.Web.Helper.Serialization
{
    public class Binary
    {
        public static byte[] Serialize(object value)
        {
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream();
                
                BinaryFormatter binFormater = new BinaryFormatter();
                binFormater.Serialize(forStream, value);

                return ((MemoryStream)forStream).ToArray();
            }
            catch (System.Exception)
            {
                return null;
            }
            finally
            {
                if (forStream != null)
                {
                    forStream.Close();
                    GC.SuppressFinalize(forStream);
                }
            }
        }

        public static T DeSerialize<T>(byte[] value)
        {
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream(value);

                BinaryFormatter binFormater = new BinaryFormatter();

                return (T)Convert.ChangeType(binFormater.Deserialize(forStream), typeof(T));
            }
            catch (System.Exception)
            {
                return default(T);
            }
            finally
            {
                if (forStream != null)
                {
                    forStream.Close();
                    GC.SuppressFinalize(forStream);
                }
            }
        }
    }
}
