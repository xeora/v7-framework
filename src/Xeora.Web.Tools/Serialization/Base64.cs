using System;

namespace Xeora.Web.Tools.Serialization
{
    public static class Base64
    {
        public static string Serialize(byte[] value) =>
            Convert.ToBase64String(value);

        public static byte[] DeSerialize(string base64data) =>
            Convert.FromBase64String(base64data);
    }
}
