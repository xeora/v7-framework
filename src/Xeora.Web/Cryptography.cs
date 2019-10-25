using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Xeora.Web
{
    public class Cryptography : Basics.ICryptography
    {
        private readonly Aes _Aes;
        
        public Cryptography(string cryptoId)
        {
            if (string.IsNullOrEmpty(cryptoId))
                throw new ArgumentException(nameof(cryptoId));
            
            this._Aes = Aes.Create();
            if (this._Aes == null) 
                throw new CryptographicException();
            
            this._Aes.Mode = CipherMode.CBC;
            this._Aes.Padding = PaddingMode.ISO10126;
            
            this.AssignCipher(cryptoId);
        }

        private void AssignCipher(string cryptoId)
        {
            byte[] cryptoBytes =
                Encoding.UTF8.GetBytes(cryptoId);
            byte[] saltBytes = {1, 2, 3, 4, 5, 6, 7, 8};
            
            Rfc2898DeriveBytes rfc = 
                new Rfc2898DeriveBytes(cryptoBytes, saltBytes, 1000);

            this._Aes.Key = rfc.GetBytes(this._Aes.KeySize / 8);
            this._Aes.IV = rfc.GetBytes(this._Aes.BlockSize / 8);
        }

        public string Encrypt(string input)
        {
            MemoryStream encrypted = null;
            
            try
            {
                ICryptoTransform encryptor =
                    this._Aes.CreateEncryptor();
                
                encrypted = new MemoryStream();
                
                CryptoStream encryptStream = null;
                try
                {
                    byte[] inputBytes =
                        Encoding.UTF8.GetBytes(input);

                    encryptStream = 
                        new CryptoStream(encrypted, encryptor, CryptoStreamMode.Write);
                    encryptStream.Write(inputBytes, 0, inputBytes.Length);
                    encryptStream.FlushFinalBlock();
                        
                    // notify session to be persist
                    Basics.Helpers.Context.Session["_sys_crypto"] =
                        Guid.NewGuid().ToString();
                }
                finally
                {
                    encryptStream?.Close();
                }

                byte[] encryptedBytes = 
                    encrypted.ToArray();
                
                StringBuilder encryptedString = 
                    new StringBuilder();
                foreach (byte encryptedByte in encryptedBytes)
                    encryptedString.Append(
                        encryptedByte.ToString("X2"));
                
                return encryptedString.ToString();
            }
            finally
            {
                encrypted?.Close();
            }
        }

        public string Decrypt(string encryptedInput)
        {
            MemoryStream decryptCache = null;
            try
            {
                ICryptoTransform decryptor =
                    this._Aes.CreateDecryptor();
                
                decryptCache = 
                    new MemoryStream();
                for (int i = 0; i < encryptedInput.Length; i += 2)
                {
                    string hex = 
                        encryptedInput.Substring(i, 2);
                    decryptCache.WriteByte(
                        byte.Parse(hex, System.Globalization.NumberStyles.HexNumber));
                }
                decryptCache.Seek(0, SeekOrigin.Begin);

                CryptoStream decryptStream = null;
                try
                {
                    StringBuilder decrypted = 
                        new StringBuilder();
                    
                    decryptStream = 
                        new CryptoStream(decryptCache, decryptor, CryptoStreamMode.Read);

                    byte[] buffer = new byte[2048];
                    int rC;
                    do
                    {
                        rC = decryptStream.Read(buffer, 0, buffer.Length);
                        
                        if (rC > 0)
                            decrypted.Append(Encoding.UTF8.GetString(buffer, 0, rC));
                    } while (rC > 0);
                    
                    return decrypted.ToString();
                }
                finally
                {
                    decryptStream?.Close();
                }
            }
            finally
            {
                decryptCache?.Close();
            }
        }
    }
}
