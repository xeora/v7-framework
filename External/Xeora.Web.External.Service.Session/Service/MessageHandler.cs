using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xeora.Web.External.Service.Session
{
    public class MessageHandler
    {
        private Stream _ResponseStream;

        public MessageHandler(ref Stream responseStream) =>
            this._ResponseStream = responseStream;

        public async void ProcessAsync(long requestID, byte[] requestBytes) =>
            await Task.Run(() => this.Process(requestID, requestBytes));

        private void Process(long requestID, byte[] requestBytes)
        {
            Stream responseStream = null;
            Stream requestStream = null;

            try
            {
                responseStream = new MemoryStream();
                requestStream = new MemoryStream(requestBytes, 0, requestBytes.Length, false);
                requestStream.Seek(0, SeekOrigin.Begin);

                BinaryReader binaryReader =
                     new BinaryReader(requestStream);

                char[] command = binaryReader.ReadChars(3);

                switch (new string(command))
                {
                    case "ACQ":
                        this.HandleACQ(requestID, ref binaryReader, ref responseStream);

                        break;
                    case "GET":
                        this.HandleGET(requestID, ref binaryReader, ref responseStream);

                        break;
                    case "SET":
                        this.HandleSET(requestID, ref binaryReader, ref responseStream);

                        break;
                    case "KYS":
                        this.HandleKYS(requestID, ref binaryReader, ref responseStream);

                        break;
                }

                Monitor.Enter(this._ResponseStream);
                try
                {
                    responseStream.Seek(0, SeekOrigin.Begin);
                    responseStream.CopyTo(this._ResponseStream);
                }
                finally
                {
                    Monitor.Exit(this._ResponseStream);
                }
            }
            catch
            {
                // Just Handle Exception
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    GC.SuppressFinalize(requestStream);
                }

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }

        private void PutHeader(long requestID, ref Stream contentStream)
        {
            long contentLength = contentStream.Position;
            contentLength -= 8; // Remove long length;

            long head = requestID << 24;
            head = head | contentLength;

            byte[] headBytes = BitConverter.GetBytes(head);

            contentStream.Seek(0, SeekOrigin.Begin);
            contentStream.Write(headBytes, 0, headBytes.Length);

            contentStream.Seek(0, SeekOrigin.End);
        }

        private void HandleACQ(long requestID, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\ACQ\SHORT\INT
             * -> \LONG\ACQ\SHORT\INT\BYTE\CHARS{BYTEVALUELENGTH}
             */

            short sessionTimeout = responseReader.ReadInt16();
            int remoteIP = responseReader.ReadInt32();
            byte sessionIDLength = responseReader.ReadByte();
            string sessionID = string.Empty;

            if (sessionIDLength > 0)
                sessionID = new string(responseReader.ReadChars(sessionIDLength));

            try
            {
                BinaryWriter binaryWriter = 
                    new BinaryWriter(responseStream);

                // Put Dummy Header
                binaryWriter.Write((long)0);

                Basics.Session.IHttpSession sessionObject;
                try
                {
                    SessionManager.Current.Acquire(remoteIP, sessionID, sessionTimeout, out sessionObject);

                    /*
                     * <- \LONG\BYTE\BYTE\CHARS{BYTEVALUELENGTH}\LONG
                     */

                    binaryWriter.Write((byte)1);
                    binaryWriter.Write((byte)sessionObject.SessionID.Length);
                    binaryWriter.Write(sessionObject.SessionID.ToCharArray());
                    binaryWriter.Write(sessionObject.Expires.Ticks);
                }
                catch
                {
                    /*
                     * <- \LONG\BYTE
                     */

                    binaryWriter.Write((byte)2);
                }

                binaryWriter.Flush();

                this.PutHeader(requestID, ref responseStream);
            }
            catch
            {
                return;
            }
        }

        private bool GetSessionObject(ref BinaryReader responseReader, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            int remoteIP = responseReader.ReadInt32();
            byte sessionIDLength = responseReader.ReadByte();
            string sessionID = string.Empty;

            if (sessionIDLength == 0)
                return false;

            sessionID = new string(responseReader.ReadChars(sessionIDLength));

            return SessionManager.Current.Obtain(remoteIP, sessionID, out sessionObject);
        }

        private void HandleGET(long requestID, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\GET\INT\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
             */

            Basics.Session.IHttpSession sessionObject;
            if (!this.GetSessionObject(ref responseReader, out sessionObject))
                return;

            byte keyLength = responseReader.ReadByte();
            string key = new string(responseReader.ReadChars(keyLength));

            if (string.IsNullOrEmpty(key))
                return;

            object value = sessionObject[key];

            try
            {
                BinaryWriter binaryWriter = 
                    new BinaryWriter(responseStream);

                /*
                 * <- \LONG\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
                 */

                // Put Dummy Header
                binaryWriter.Write((long)0);

                binaryWriter.Write(keyLength);
                binaryWriter.Write(key.ToCharArray());

                if (value != null)
                {
                    byte[] valueBytes = (byte[])value;

                    binaryWriter.Write(valueBytes.Length);
                    binaryWriter.Write(valueBytes, 0, valueBytes.Length);
                }
                else
                    binaryWriter.Write((int)0);

                binaryWriter.Flush();

                this.PutHeader(requestID, ref responseStream);
            }
            catch
            {
                return;
            }
        }

        private void HandleSET(long requestID, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\SET\INT\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
             */

            Basics.Session.IHttpSession sessionObject;
            if (!this.GetSessionObject(ref responseReader, out sessionObject))
                return;

            byte keyLength = responseReader.ReadByte();
            string key = new string(responseReader.ReadChars(keyLength));

            int valueLength = responseReader.ReadInt32();
            byte[] valueBytes = responseReader.ReadBytes(valueLength);

            if (string.IsNullOrEmpty(key))
                return;

            sessionObject[key] = valueBytes;

            try
            {
                BinaryWriter binaryWriter = 
                    new BinaryWriter(responseStream);

                /*
                 * <- \LONG\BYTE
                 */

                // Put dummy header
                binaryWriter.Write((long)0);

                binaryWriter.Write((byte)1);
                binaryWriter.Flush();

                this.PutHeader(requestID, ref responseStream);
            }
            catch
            {
                return;
            }
        }

        private void HandleKYS(long requestID, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\KYS\INT\BYTE\CHARS{BYTEVALUELENGTH}
             */

            Basics.Session.IHttpSession sessionObject;
            if (!this.GetSessionObject(ref responseReader, out sessionObject))
                return;

            try
            {
                BinaryWriter binaryWriter = 
                    new BinaryWriter(responseStream);

                // Put dummy header
                binaryWriter.Write((long)0);

                try
                {
                    /*
                     * <- \LONG\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}...
                     */

                    foreach (string key in sessionObject.Keys)
                    {
                        binaryWriter.Write((byte)key.Length);
                        binaryWriter.Write(key.ToCharArray());
                    }
                }
                catch
                { /* Just Handle Exceptions*/ }

                binaryWriter.Flush();

                this.PutHeader(requestID, ref responseStream);
            }
            catch
            {
                return;
            }
        }
    }
}
