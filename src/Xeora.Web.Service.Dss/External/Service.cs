using System;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Exceptions;

namespace Xeora.Web.Service.Dss.External
{
    internal class Service : Basics.Dss.IDss, IService
    {
        private readonly RequestHandler _RequestHandler;
        private readonly ResponseHandler _ResponseHandler;

        public Service(ref RequestHandler requestHandler, ref ResponseHandler responseHandler, string uniqueId, bool reusing, DateTime expireDate)
        {
            this._RequestHandler = requestHandler;
            this._ResponseHandler = responseHandler;

            this.UniqueId = uniqueId;
            this.Reusing = reusing;
            this.Expires = expireDate;
        }

        public string UniqueId { get; }
        public bool Reusing { get; }
        public DateTime Expires { get; }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            return this._Get(key);
        }
        
        public void Set(string key, object value, string lockCode = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (key.Length > 128)
                throw new OverflowException("key can not be longer than 128 characters");
                
            this._Set(key, value, lockCode);
        }

        public string Lock(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
                
            if (key.Length > 128)
                throw new OverflowException("key can not be longer than 128 characters");
            
            return this._Lock(key);
        }

        public void Release(string key, string lockCode) => this._Release(key, lockCode);
        public string[] Keys => this.GetKeys();

        private object _Get(string key)
        {
            long requestId;

            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> GET\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("GET".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Flush();

                requestId = 
                    this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray()) ?? -1;
                if (requestId == -1) return null;
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }

            byte[] responseBytes = 
                this._ResponseHandler.WaitForMessage(requestId);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            
            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                responseStream = 
                    new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
                 */

                byte remoteResult = 
                    binaryReader.ReadByte();

                switch (remoteResult)
                {
                    case 0:
                        byte remoteKeyLength = 
                            binaryReader.ReadByte();
                        string remoteKey = 
                            new string(binaryReader.ReadChars(remoteKeyLength));
                        
                        int remoteValueLength = binaryReader.ReadInt32();
                        byte[] remoteValueBytes =
                            binaryReader.ReadBytes(remoteValueLength);

                        if (string.CompareOrdinal(remoteKey, key) != 0)
                            throw new DssCommandException();
                        
                        return Tools.Serialization.Binary.DeSerialize(remoteValueBytes);
                    default:
                        throw new DssCommandException();
                }
            }
            finally
            {
                binaryReader?.Dispose();
                responseStream?.Dispose();
            }
        }

        private void _Set(string key, object value, string lockCode)
        {
            if (string.IsNullOrEmpty(lockCode))
                lockCode = string.Empty;
            
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> SET\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
                 */

                byte[] valueBytes = 
                    Tools.Serialization.Binary.Serialize(value) ?? Array.Empty<byte>();
                if (valueBytes.Length > 16777000)
                    throw new OverflowException("Value is too big to store");

                binaryWriter.Write("SET".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Write((byte)lockCode.Length);
                binaryWriter.Write(lockCode.ToCharArray());
                binaryWriter.Write(valueBytes.Length);
                binaryWriter.Write(valueBytes, 0, valueBytes.Length);
                binaryWriter.Flush();

                long requestId =
                    this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray()) ?? -1;
                if (requestId == -1) return;

                byte[] responseBytes = this._ResponseHandler.WaitForMessage(requestId);
                if (responseBytes == null || responseBytes.Length == 0)
                    return;

                switch (responseBytes[0])
                {
                    case 0:
                        return;
                    case 30:
                        throw new KeyLockedException();
                    default:
                        throw new DssCommandException();                        
                }
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }
        }
        
        private string _Lock(string key)
        {
            long requestId;

            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> LCK\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("LCK".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Flush();

                requestId = 
                    this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray()) ?? -1;
                if (requestId == -1) return null;
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }

            byte[] responseBytes = 
                this._ResponseHandler.WaitForMessage(requestId);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            
            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                responseStream = 
                    new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                byte remoteResult = 
                    binaryReader.ReadByte();

                switch (remoteResult)
                {
                    case 0:
                        byte remoteKeyLength = binaryReader.ReadByte();
                        string remoteKey = 
                            new string(binaryReader.ReadChars(remoteKeyLength));
                        
                        int remoteLockCodeLength = binaryReader.ReadByte();
                        string remoteLockCode =
                            new string(binaryReader.ReadChars(remoteLockCodeLength));

                        if (string.CompareOrdinal(remoteKey, key) != 0)
                            throw new DssCommandException();
                        
                        return remoteLockCode;
                    default:
                        throw new DssCommandException();
                }
            }
            finally
            {
                binaryReader?.Dispose();
                responseStream?.Dispose();
            }
        }

        private void _Release(string key, string lockCode)
        {            
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> RLS\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("RLS".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Write((byte)lockCode.Length);
                binaryWriter.Write(lockCode.ToCharArray());
                binaryWriter.Flush();

                long requestId =
                    this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray()) ?? -1;
                if (requestId == -1) return;

                byte[] responseBytes = this._ResponseHandler.WaitForMessage(requestId);
                if (responseBytes == null || responseBytes.Length == 0)
                    return;

                if (responseBytes[0] != 0)
                    throw new DssCommandException();
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }
        }

        private string[] GetKeys()
        {
            List<string> keys = new List<string>();
            
            long requestId;

            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> KYS\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("KYS".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Flush();

                requestId = this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray()) ?? -1;
                if (requestId == -1) return keys.ToArray();
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }

            byte[] responseBytes = 
                this._ResponseHandler.WaitForMessage(requestId);
            if (responseBytes == null || responseBytes.Length == 0)
                return keys.ToArray();
            
            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                responseStream = 
                    new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}...
                 */

                byte remoteResult = 
                    binaryReader.ReadByte();

                switch (remoteResult)
                {
                    case 0:
                        do
                        {
                            byte remoteKeyLength = 
                                binaryReader.ReadByte();
                            if (remoteKeyLength == 0) break;
                            
                            string remoteKey =
                                new string(binaryReader.ReadChars(remoteKeyLength));
                    
                            keys.Add(remoteKey);
                        } while (binaryReader.PeekChar() > -1);
                        
                        return keys.ToArray();
                    default:
                        throw new DssCommandException();
                }
            }
            finally
            {
                binaryReader?.Dispose();
                responseStream?.Dispose();
            }
        }


        public bool IsExpired => DateTime.Compare(DateTime.UtcNow, this.Expires) > 0;

        public void Extend()
        {
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> EXT\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("EXT".ToCharArray());
                binaryWriter.Write((byte)this.UniqueId.Length);
                binaryWriter.Write(this.UniqueId.ToCharArray());
                binaryWriter.Flush();

                if (this._RequestHandler == null)
                    throw new ExternalCommunicationException();
                
                this._RequestHandler?.MakeRequest(((MemoryStream)requestStream).ToArray());
            }
            finally
            {
                binaryWriter?.Dispose();
                requestStream?.Dispose();
            }
        }
    }
}
