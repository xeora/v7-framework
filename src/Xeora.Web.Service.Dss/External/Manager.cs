using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class Manager : IManager
    {
        private readonly object _ConnectionLock;
        private TcpClient _DssServiceClient;

        private readonly IPEndPoint _ServiceEndPoint;

        private RequestHandler _RequestHandler;
        private ResponseHandler _ResponseHandler;

        public Manager(IPEndPoint serviceEndPoint)
        {
            this._ConnectionLock = new object();
            this._DssServiceClient = null;

            this._ServiceEndPoint = serviceEndPoint;
        }

        private void Reset()
        {
            this._DssServiceClient?.Close();
            this._DssServiceClient?.Dispose();
            this._DssServiceClient = null;

            this._ResponseHandler = null;
            this._RequestHandler = null;
        }

        private void MakeConnection()
        {
            Monitor.Enter(this._ConnectionLock);
            try
            {
                if (this._DssServiceClient != null && this._DssServiceClient.Client.Connected)
                    return;

                this._DssServiceClient?.Dispose();

                this._DssServiceClient = new TcpClient();
                this._DssServiceClient.Connect(this._ServiceEndPoint);

                if (!this._DssServiceClient.Client.Connected)
                    throw new Exceptions.ExternalCommunicationException();
            }
            catch (Exceptions.ExternalCommunicationException)
            {
                this.Reset();
                throw;
            }
            finally
            {
                Monitor.Exit(this._ConnectionLock);
            }

            this._RequestHandler = new RequestHandler(ref this._DssServiceClient);
            this._ResponseHandler = new ResponseHandler(ref this._DssServiceClient);
            this._ResponseHandler.StartHandler();
        }

        public void Reserve(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject)
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
                binaryWriter.Write((byte) uniqueId.Length);
                binaryWriter.Write(uniqueId.ToCharArray());
                binaryWriter.Flush();

                long requestId =
                    _RequestHandler?.MakeRequest(((MemoryStream) requestStream).ToArray()) ?? -1;
                if (requestId == -1)
                {
                    reservationObject = null;
                    return;
                }

                byte[] responseBytes =
                    this._ResponseHandler.WaitForMessage(requestId);

                this.ParseResponse(responseBytes, out reservationObject);
            }
            catch (Exceptions.ExternalCommunicationException)
            {
                this.Reset();
                throw;
            }
            finally
            {
                binaryWriter?.Close();
                requestStream?.Close();
            }
        }

        private void ParseResponse(byte[] responseBytes, out Basics.Dss.IDss reservationObject)
        {
            reservationObject = null;

            if (responseBytes == null)
                return;
            
            Stream contentStream = 
                new MemoryStream(responseBytes, 0, responseBytes.Length, false);
            BinaryReader binaryReader = 
                new BinaryReader(contentStream);

            /*
             * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\LONG
             * <- \BYTE
             */

            if (binaryReader.ReadByte() == 2)
                throw new Exceptions.ReservationCreationException();

            byte reservationIdLength = binaryReader.ReadByte();
            string reservationId =
                new string(binaryReader.ReadChars(reservationIdLength));
            byte reusing = binaryReader.ReadByte();
            DateTime expireDate =
                new DateTime(binaryReader.ReadInt64());

            reservationObject = new Service(ref this._RequestHandler, ref this._ResponseHandler, reservationId, reusing == 1, expireDate);
        }
    }
}
