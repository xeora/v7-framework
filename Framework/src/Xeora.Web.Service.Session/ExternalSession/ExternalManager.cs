using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Session
{
    public class ExternalManager : IHttpSessionManager
    {
        private object _ConnectionLock;
        private TcpClient _SessionServiceClient;

        private IPEndPoint _ServiceEndPoint;
        private short _SessionTimeout = 20;

        private RequestHandler _RequestHandler;
        private ResponseHandler _ResponseHandler;

        public ExternalManager(IPEndPoint serviceEndPoint, short sessionTimeout)
        {
            this._ConnectionLock = new object();
            this._SessionServiceClient = null;

            this._ServiceEndPoint = serviceEndPoint;
            this._SessionTimeout = sessionTimeout;
        }

        private void MakeConnection()
        {
            Monitor.Enter(this._ConnectionLock);
            try
            {
                if (this._SessionServiceClient != null &&
                   this._SessionServiceClient.Connected)
                    return;
                
                this._SessionServiceClient = new TcpClient();
                this._SessionServiceClient.Connect(this._ServiceEndPoint);

                if (!this._SessionServiceClient.Connected)
                    throw new ExternalCommunicationException();
            }
            finally
            {
                Monitor.Exit(this._ConnectionLock);
            }

            this._RequestHandler = new RequestHandler(ref this._SessionServiceClient);
            this._ResponseHandler = new ResponseHandler(ref this._SessionServiceClient);
            this._ResponseHandler.HandleAsync();
        }

        public void Acquire(IPAddress remoteIP, string sessionID, out Basics.Session.IHttpSession sessionObject)
        {
            this.MakeConnection();

            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> \LONG\ACQ\SHORT\INT
                 * -> \LONG\ACQ\SHORT\INT\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("ACQ".ToCharArray());
                binaryWriter.Write(this._SessionTimeout);
                binaryWriter.Write(remoteIP.GetAddressBytes());
                binaryWriter.Write((byte)sessionID.Length);
                binaryWriter.Write(sessionID.ToCharArray());
                binaryWriter.Flush();

                long requestID =
                    this._RequestHandler.MakeRequest(((MemoryStream)requestStream).ToArray());

                byte[] responseBytes =
                    this._ResponseHandler.WaitForMessage(requestID);

                this.ParseResponse(responseBytes, remoteIP, out sessionObject);
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

        private void ParseResponse(byte[] responseBytes, IPAddress remoteIP, out Basics.Session.IHttpSession sessionObject)
        {
            sessionObject = null;

            if (responseBytes == null)
                return;
            
            Stream contentStream = null;
            BinaryReader binaryReader = null;

            contentStream = new MemoryStream(responseBytes, 0, responseBytes.Length, false);
            binaryReader = new BinaryReader(contentStream);

            /*
             * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\LONG
             * <- \BYTE
             */

            if (binaryReader.ReadByte() == 2)
                throw new SessionCreationException();

            byte sessionIDLength = binaryReader.ReadByte();
            string sessionID =
                new string(binaryReader.ReadChars(sessionIDLength));
            DateTime expireDate =
                new DateTime(binaryReader.ReadInt64());

            sessionObject = new ExternalSession(ref this._RequestHandler, ref this._ResponseHandler, remoteIP, sessionID, expireDate);
        }
    }
}
