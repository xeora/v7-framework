namespace Xeora.Web.Basics
{
    public interface ICryptography
    {
        string Encrypt(string input);
        string Decrypt(string encryptedInput);
    }
}