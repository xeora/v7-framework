using System;
using System.IO;

namespace Xeora.Web.External.Service.Session
{
    public class MessageHandler
    {
        private Stream _ResponseStream;
        private Basics.Session.IHttpSession _HttpSession;

        public MessageHandler(ref Stream responseStream)
        {
            this._ResponseStream = responseStream;
            this._HttpSession = null;
        }

        public void Process(ref Stream requestStream)
        {
            requestStream.Seek(0, SeekOrigin.Begin);

            BinaryReader binaryReader =
                 new BinaryReader(requestStream);

            char[] command = binaryReader.ReadChars(3);

            switch (new string(command))
            {
                case "ACQ":
                    this.HandleACQ(ref binaryReader);

                    break;
                case "GET":
                    this.HandleGET(ref binaryReader);

                    break;
                case "SET":
                    this.HandleSET(ref binaryReader);

                    break;
                case "KYS":
                    this.HandleKYS(ref binaryReader);

                    break;
            }

            // Consume Terminator
            binaryReader.ReadByte();
        }

        private void HandleACQ(ref BinaryReader responseReader)
        {
            /*
             * -> ACQ\SHORT\INT\0
             * -> ACQ\SHORT\INT\BYTE\CHARS{BYTEVALUELENGTH}\0
             */

            short sessionTimeout = responseReader.ReadInt16();
            int remoteIP = responseReader.ReadInt32();
            byte sessionIDLength = responseReader.ReadByte();
            string sessionID = string.Empty;

            if (sessionIDLength > 0)
                sessionID = new string(responseReader.ReadChars(sessionIDLength));

            BinaryWriter binaryWriter = null;
            Stream responseStream = null;

            try
            {
                SessionManager.Current.Acquire(remoteIP, sessionID, sessionTimeout, out this._HttpSession);

                responseStream = new MemoryStream();
                binaryWriter = new BinaryWriter(responseStream);

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\LONG\0
                 */

                binaryWriter.Write((byte)1);
                binaryWriter.Write((byte)this._HttpSession.SessionID.Length);
                binaryWriter.Write(this._HttpSession.SessionID.ToCharArray());
                binaryWriter.Write(this._HttpSession.Expires.Ticks);
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                responseStream.Seek(0, SeekOrigin.Begin);
                responseStream.CopyTo(this._ResponseStream);
            }
            catch
            {
                this._ResponseStream.Write(new byte[] { 2, 0 }, 0, 2);
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }

        private void HandleGET(ref BinaryReader responseReader)
        {
            /*
             * -> GET\BYTE\CHARS{BYTEVALUELENGTH}\0
             */

            if (this._HttpSession == null)
                return;

            byte keyLength = responseReader.ReadByte();
            string key = new string(responseReader.ReadChars(keyLength));

            if (string.IsNullOrEmpty(key))
                return;

            object value = this._HttpSession[key];

            BinaryWriter binaryWriter = null;
            Stream responseStream = null;

            try
            {
                responseStream = new MemoryStream();
                binaryWriter = new BinaryWriter(responseStream);

                /*
                 * <- \BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}\0
                 */

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
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                responseStream.Seek(0, SeekOrigin.Begin);
                responseStream.CopyTo(this._ResponseStream);
            }
            catch
            {
                return;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }

        private void HandleSET(ref BinaryReader responseReader)
        {
            /*
             * -> SET\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}\0
             */

            if (this._HttpSession == null)
                return;

            byte keyLength = responseReader.ReadByte();
            string key = new string(responseReader.ReadChars(keyLength));

            int valueLength = responseReader.ReadInt32();
            byte[] valueBytes = responseReader.ReadBytes(valueLength);

            if (string.IsNullOrEmpty(key))
                return;

            this._HttpSession[key] = valueBytes;

            /*
             * <- \BYTE\0
             */

            this._ResponseStream.Write(new byte[] { 1, 0 }, 0, 2);
        }

        private void HandleKYS(ref BinaryReader responseReader)
        {
            if (this._HttpSession == null)
                return;

            BinaryWriter binaryWriter = null;
            Stream responseStream = null;

            try
            {
                responseStream = new MemoryStream();
                binaryWriter = new BinaryWriter(responseStream);

                /*
                 * <- [KEY][KEY][KEY]...\0
                 */

                foreach (string key in this._HttpSession.Keys)
                {
                    binaryWriter.Write((byte)key.Length);
                    binaryWriter.Write(key.ToCharArray());
                }
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                responseStream.Seek(0, SeekOrigin.Begin);
                responseStream.CopyTo(this._ResponseStream);
            }
            catch 
            {
                this._ResponseStream.Write(new byte[] { 0 }, 0, 1);
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();

                if (responseStream != null)
                {
                    responseStream.Close();
                    GC.SuppressFinalize(responseStream);
                }
            }
        }
    }
}
