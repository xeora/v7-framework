using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace Xeora.Web.Service.DSS
{
    internal class ExternalDSS : Basics.DSS.IDSS, IDSSService
    {
        private RequestHandler _RequestHandler;
        private ResponseHandler _ResponseHandler;

        public ExternalDSS(ref RequestHandler requestHandler, ref ResponseHandler responseHandler, string uniqueID, DateTime expireDate)
        {
            this._RequestHandler = requestHandler;
            this._ResponseHandler = responseHandler;

            this.UniqueID = uniqueID;
            this.Expires = expireDate;
        }

        public object this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                
                return this.Get(key);
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                if (key.Length > 128)
                    throw new OverflowException("key can not be longer than 128 characters");
                
                this.Set(key, value);
            }
        }

        public string UniqueID { get; private set; }
        public DateTime Expires { get; private set; }
        public string[] Keys => this.GetKeys();

        private object Get(string key)
        {
            byte[] responseBytes = null;

            long requestID;

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
                binaryWriter.Write((byte)this.UniqueID.Length);
                binaryWriter.Write(this.UniqueID.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Flush();

                requestID = this._RequestHandler.MakeRequest(((MemoryStream)requestStream).ToArray());
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (requestStream != null)
                {
                    requestStream.Close();
                    GC.SuppressFinalize(requestStream);
                }
            }

            responseBytes = this._ResponseHandler.WaitForMessage(requestID);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            
            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                responseStream = new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
                 */

                byte remoteKeyLength = binaryReader.ReadByte();
                string remoteKey = 
                    new string(binaryReader.ReadChars(remoteKeyLength));

                int remoteValueLength = binaryReader.ReadInt32();
                byte[] remoteValueBytes = 
                    binaryReader.ReadBytes(remoteValueLength);

                if (string.Compare(remoteKey, key) != 0)
                    return null;

                return this.DeSerialize(remoteValueBytes);
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }

        private void Set(string key, object value)
        {            
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> SET\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
                 */

                byte[] valueBytes = this.Serialize(value);
                if (valueBytes.Length > 16777000)
                    throw new OverflowException("Value is too big to store");

                binaryWriter.Write("SET".ToCharArray());
                binaryWriter.Write((byte)this.UniqueID.Length);
                binaryWriter.Write(this.UniqueID.ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Write(valueBytes.Length);
                binaryWriter.Write(valueBytes, 0, valueBytes.Length);
                binaryWriter.Flush();

                long requestID =
                    this._RequestHandler.MakeRequest(((MemoryStream)requestStream).ToArray());

                byte[] responseBytes = this._ResponseHandler.WaitForMessage(requestID);
                if (responseBytes == null || responseBytes.Length == 0)
                    return;

                if (responseBytes[0] != 1)
                    throw new DSSValueException();
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (requestStream != null)
                {
                    requestStream.Close();
                    GC.SuppressFinalize(requestStream);
                }
            }
        }

        private string[] GetKeys()
        {
            List<string> keys = new List<string>();

            byte[] responseBytes = null;
            long requestID;

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
                binaryWriter.Write((byte)this.UniqueID.Length);
                binaryWriter.Write(this.UniqueID.ToCharArray());
                binaryWriter.Flush();

                requestID = this._RequestHandler.MakeRequest(((MemoryStream)requestStream).ToArray());
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (requestStream != null)
                {
                    requestStream.Close();
                    GC.SuppressFinalize(requestStream);
                }
            }

            responseBytes = this._ResponseHandler.WaitForMessage(requestID);
            if (responseBytes == null || responseBytes.Length == 0)
                return keys.ToArray();
            
            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                responseStream = new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}...
                 */

                do
                {
                    byte remoteKeyLength = binaryReader.ReadByte();
                    if (remoteKeyLength == 0)
                        break;
                    
                    keys.Add(new string(binaryReader.ReadChars(remoteKeyLength)));
                } while (binaryReader.PeekChar() > -1);
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
            finally
            {
                if (binaryReader != null)
                    binaryReader.Close();

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }

            return keys.ToArray();
        }

        private byte[] Serialize(object value)
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
                return new byte[] { };
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

        private object DeSerialize(byte[] value)
        {
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream(value);

                BinaryFormatter binFormater = new BinaryFormatter();

                return binFormater.Deserialize(forStream);
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

        public bool IsExpired => throw new NotImplementedException();
        public void Extend() =>
            throw new NotImplementedException();
    }
}
