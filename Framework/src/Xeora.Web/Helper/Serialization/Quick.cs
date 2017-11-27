namespace Xeora.Web.Helper.Serialization
{
    public class Quick
    {
        public static string BinaryToBase64Serialize(object input)
        {
            byte[] serializedBytes = Binary.Serialize(input);

            return Base64.Serialize(serializedBytes);
        }

        public static T Base64ToBinaryDeSerialize<T>(string base64data)
        {
            byte[] serializedBytes = Base64.DeSerialize(base64data);

            return Binary.DeSerialize<T>(serializedBytes);
        }
    }
}
