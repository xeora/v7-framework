using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.DSS
{
    public class ExternalManager : IDSSManager
    {
        private readonly object _ConnectionLock;
        private TcpClient _DSSServiceClient;

        private readonly IPEndPoint _ServiceEndPoint;

        private RequestHandler _RequestHandler;
        private ResponseHandler _ResponseHandler;

        public ExternalManager(IPEndPoint serviceEndPoint)
        {
            this._ConnectionLock = new object();
            this._DSSServiceClient = null;

            this._ServiceEndPoint = serviceEndPoint;
        }

        private void MakeConnection()
        {
            Monitor.Enter(this._ConnectionLock);
            try
            {
                if (this._DSSServiceClient != null &&
                   this._DSSServiceClient.Connected)
                    return;
                
                this._DSSServiceClient = new TcpClient();
                this._DSSServiceClient.Connect(this._ServiceEndPoint);

                if (!this._DSSServiceClient.Connected)
                    throw new ExternalCommunicationException();
            }
            finally
            {
                Monitor.Exit(this._ConnectionLock);
            }

            this._RequestHandler = new RequestHandler(ref this._DSSServiceClient);
            this._ResponseHandler = new ResponseHandler(ref this._DSSServiceClient);
            this._ResponseHandler.HandleAsync();
        }

        public void Reserve(string uniqueID, int reservationTimeout, out Basics.DSS.IDSS reservationObject)
        {
            this.MakeConnection();

            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> \LONG\ACQ\SHORT
                 * -> \LONG\ACQ\SHORT\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                binaryWriter.Write("ACQ".ToCharArray());
                binaryWriter.Write(reservationTimeout);
                binaryWriter.Write((byte)uniqueID.Length);
                binaryWriter.Write(uniqueID.ToCharArray());
                binaryWriter.Flush();

                long requestID =
                    this._RequestHandler.MakeRequest(((MemoryStream)requestStream).ToArray());

                byte[] responseBytes =
                    this._ResponseHandler.WaitForMessage(requestID);

                this.ParseResponse(responseBytes, out reservationObject);
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

        private void ParseResponse(byte[] responseBytes, out Basics.DSS.IDSS reservationObject)
        {
            reservationObject = null;

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
                throw new ReservationCreationException();

            byte reservationIDLength = binaryReader.ReadByte();
            string reservationID =
                new string(binaryReader.ReadChars(reservationIDLength));
            DateTime expireDate =
                new DateTime(binaryReader.ReadInt64());

            reservationObject = new ExternalDSS(ref this._RequestHandler, ref this._ResponseHandler, reservationID, expireDate);
        }
    }
}
