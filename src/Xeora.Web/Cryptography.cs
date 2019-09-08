using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Xeora.Web
{
    public class Cryptography
    {
        private static Cryptography _Current;
        private static readonly object CryptographyLock = 
            new object();
        public static Cryptography Current
        {
            get
            {
                if (Cryptography._Current != null)
                    return Cryptography._Current;
                
                Monitor.Enter(Cryptography.CryptographyLock);
                try
                {
                    Cryptography._Current = 
                        new Cryptography(Basics.Configurations.Xeora.Application.Main.PhysicalRoot);
                    return Cryptography._Current;
                }
                finally
                {
                    Monitor.Exit(Cryptography.CryptographyLock);
                }
            }
        }

        private readonly ICryptoTransform _Encryptor;
        private readonly ICryptoTransform _Decryptor;
        
        private Cryptography(string applicationPath)
        {
            if (string.IsNullOrEmpty(applicationPath))
                throw new ArgumentException(nameof(applicationPath));
            
            Aes aes = Aes.Create();
            if (aes == null) 
                throw new CryptographicException();
            
            aes.Key = 
                this.GenerateKey(applicationPath);
            this.AssignVector(ref aes);
            aes.Padding = PaddingMode.PKCS7;

            this._Encryptor = 
                aes.CreateEncryptor();
            this._Decryptor =
                aes.CreateDecryptor();
        }

        private byte[] GenerateKey(string applicationPath)
        {
            byte[] applicationPathBytes =
                Encoding.UTF8.GetBytes(applicationPath);
            
            MD5 md5 = 
                MD5.Create();
            byte[] key = 
                md5.ComputeHash(applicationPathBytes);
            
            byte[] modKey = 
                new byte[key.Length * 2];
            Array.Copy(key, 0, modKey, 16, key.Length);
            Array.Reverse(key); 
            Array.Copy(key, 0, modKey, 0, key.Length);

            return modKey;
        }
        
        private void AssignVector(ref Aes aes)
        {
            byte[] key = aes.Key;
            byte[] vector = 
                new byte[aes.BlockSize / 8];
            
            if (key.Length < vector.Length)
                for (int i = 0; i < vector.Length; i++)
                    vector[i] = key[i % key.Length];
            else
                Array.Copy(key, 0, vector, 0, vector.Length);

            aes.IV = vector;
        }

        public string Encrypt(string input)
        {
            MemoryStream encrypted = null;
            
            try
            {
                encrypted = new MemoryStream();
                
                CryptoStream encryptStream = null;
                try
                {
                    encryptStream = 
                        new CryptoStream(encrypted, this._Encryptor, CryptoStreamMode.Write);
                    
                    StreamWriter writer = null;
                    try
                    {
                        writer = 
                            new StreamWriter(encryptStream, Encoding.UTF8);
                        writer.Write(input);
                    }
                    finally
                    {
                        writer?.Close();
                    }
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
                    decryptStream = 
                        new CryptoStream(decryptCache, this._Decryptor, CryptoStreamMode.Read);

                    StreamReader reader = null;
                    try
                    {
                        reader = 
                            new StreamReader(decryptStream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                    finally
                    {
                        reader?.Close();
                    }
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
