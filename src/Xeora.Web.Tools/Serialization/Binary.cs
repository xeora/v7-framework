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
            catch (Exception e)
            {
                Basics.Console.Push(
                    "Bin. Serializer Exception...", 
                    e.Message, 
                    e.ToString(), 
                    false, 
                    true,
                    type: Basics.Console.Type.Error);
                
                return null;
            }
            finally
            {
                forStream?.Close();
            }
        }

        public static T DeSerialize<T>(byte[] value)
        {
            if (value == null || value.Length == 0) 
                return default;
            
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream(value);

                BinaryFormatter binFormatter = new BinaryFormatter();

                return (T)Convert.ChangeType(binFormatter.Deserialize(forStream), typeof(T));
            }
            catch (Exception e)
            {
                Basics.Console.Push(
                    "Bin. Deserializer Exception...", 
                    e.Message, 
                    e.ToString(), 
                    false, 
                    true,
                    type: Basics.Console.Type.Error);
                
                return default;
            }
            finally
            {
                forStream?.Close();
            }
        }
    }
}
