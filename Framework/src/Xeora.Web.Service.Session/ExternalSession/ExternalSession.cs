using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Xeora.Web.Service.Session
{
    internal class ExternalSession : Basics.Session.IHttpSession, IHttpSessionService
    {
        private TcpClient _RemoteClient;

        public ExternalSession(ref TcpClient remoteClient, string sessionID, DateTime expireDate)
        {
            this._RemoteClient = remoteClient;
            this._RemoteClient.ReceiveTimeout = 100; // 100 ms

            this.SessionID = sessionID;
            this.Expires = expireDate;
        }

        public object this[string key]
        {
            get => this.Get(key);
            set => this.Set(key, value);
        }

        public string SessionID { get; private set; }
        public DateTime Expires { get; private set; }
        public string[] Keys => this.GetKeys();

        private object Get(string key)
        {
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> GET\BYTE\CHARS{BYTEVALUELENGTH}\0
                 */

                binaryWriter.Write("GET".ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                this.WriteSocketFrom(ref requestStream);
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

            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                this.ReadSocketInto(ref responseStream);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}\0
                 */

                byte remoteKeyLength = binaryReader.ReadByte();
                string remoteKey = 
                    new string(binaryReader.ReadChars(remoteKeyLength));

                int remoteValueLength = binaryReader.ReadInt32();
                byte[] remoteValueBytes = 
                    binaryReader.ReadBytes(remoteValueLength);

                // Consume Terminator
                binaryReader.ReadByte();

                if (string.Compare(remoteKey, key) != 0)
                    return null;

                return this.DeSerialize(remoteValueBytes);
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
                 * -> SET\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}\0
                 */

                byte[] valueBytes = this.Serialize(value);

                binaryWriter.Write("SET".ToCharArray());
                binaryWriter.Write((byte)key.Length);
                binaryWriter.Write(key.ToCharArray());
                binaryWriter.Write(valueBytes.Length);
                binaryWriter.Write(valueBytes, 0, valueBytes.Length);
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                this.WriteSocketFrom(ref requestStream);
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

            Stream responseStream = null;

            try
            {
                // Consume Stream
                this.ReadSocketInto(ref responseStream);

                /*
                 * <- \BYTE\0
                 */

                byte[] buffer = new byte[2];
                responseStream.Read(buffer, 0, buffer.Length);

                if (buffer[0] != 1)
                    throw new SessionValueException();
            }
            catch (SessionValueException)
            {
                throw;
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }

        private string[] GetKeys()
        {
            List<string> keys = new List<string>();

            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> KYS\0
                 */

                binaryWriter.Write("KYS".ToCharArray());
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                this.WriteSocketFrom(ref requestStream);
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

            // Parse Response
            BinaryReader binaryReader = null;
            Stream responseStream = null;

            try
            {
                this.ReadSocketInto(ref responseStream);
                binaryReader = new BinaryReader(responseStream);

                /*
                 * <- \BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}...\0
                 */

                do
                {
                    byte remoteKeyLength = binaryReader.ReadByte();
                    if (remoteKeyLength == 0)
                        break;
                    
                    keys.Add(new string(binaryReader.ReadChars(remoteKeyLength)));
                } while (true);
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

        private void WriteSocketFrom(ref Stream requestStream)
        {
            requestStream.Seek(0, SeekOrigin.Begin);
            requestStream.CopyTo(this._RemoteClient.GetStream());
        }

        private void ReadSocketInto(ref Stream responseStream)
        {
            if (responseStream == null)
                responseStream = new MemoryStream();
            Stream remoteStream = this._RemoteClient.GetStream();

            byte[] buffer = new byte[1024];
            int bR = 0;
            do
            {
                bR = remoteStream.Read(buffer, 0, buffer.Length);

                if (bR > 0)
                {
                    responseStream.Write(buffer, 0, bR);

                    if (buffer[bR - 1] == 0)
                        break;
                }
            } while (true);

            responseStream.Seek(0, SeekOrigin.Begin);
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

        public void Complete()
        {
            /*
             * -> \0
             */

            if (this._RemoteClient.Connected)
            {
                this._RemoteClient.GetStream().Write(new byte[] { 0 }, 0, 1);
                this._RemoteClient.Close();
            }
        }
    }
}
