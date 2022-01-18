using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class Manager : IManager
    {
        private const int WAIT_LIMIT = 100;
        
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
                if (this._DssServiceClient != null && this._DssServiceClient.Connected)
                    return;

                this._DssServiceClient?.Dispose();

                this._DssServiceClient = new TcpClient();
                this._DssServiceClient.Connect(this._ServiceEndPoint);

                if (!this._DssServiceClient.Connected)
                    throw new Exceptions.ExternalCommunicationException();

                if (!this.Validate(ref this._DssServiceClient))
                {
                    this._DssServiceClient.Dispose();
                    this._DssServiceClient = null;
                    
                    throw new Exceptions.ExternalCommunicationValidationException();
                }

                this._DssServiceClient.ReceiveTimeout = 0; // No timeout
                
                this._RequestHandler = new RequestHandler(ref this._DssServiceClient);
                this._ResponseHandler = 
                    new ResponseHandler(ref this._DssServiceClient, ex =>
                    {
                        if (ex is SocketException)
                            this.Reset();
                        else
                            Basics.Console.Push("SYSTEM ERROR", ex.Message, ex.ToString(), false, true, type: Basics.Console.Type.Error);
                    });
                this._ResponseHandler.StartHandler();
            }
            // Exceptions.ExternalCommunicationException
            // Exceptions.ExternalCommunicationValidationException
            catch
            {
                this.Reset();
                throw;
            }
            finally
            {
                Monitor.Exit(this._ConnectionLock);
            }

            ThreadPool.QueueUserWorkItem(this.EchoLoop);
        }
        
        private bool Validate(ref TcpClient dssServiceClient)
        {
            SpinWait spinWait = new SpinWait();

            dssServiceClient.ReceiveTimeout = 5000; // 5 seconds
            
            Stream remoteStream =
                dssServiceClient.GetStream();
            
            int length =
                remoteStream.ReadByte();
            byte[] code = new byte[length];
            int total = 0;
            
            do
            {
                if (spinWait.Count >= WAIT_LIMIT) return false;
                
                int bR = remoteStream.Read(code, total, code.Length - total);
                if (bR == 0)
                {
                    spinWait.SpinOnce();
                    continue;
                }

                total += bR;
                spinWait.Reset();
            } while (length != total);

            remoteStream.Write(code, 0, code.Length);
            spinWait.Reset();
            
            byte[] approved = new byte[1];
            do
            {
                if (spinWait.Count >= WAIT_LIMIT) return false;
                
                int bR = remoteStream.Read(approved, 0, approved.Length);
                if (bR == 1) return approved[0] == 1;

                spinWait.SpinOnce();
            } while (true);
        }
        
        private void EchoLoop(object state)
        {
            // Make Request
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;
            
            do
            {
                try
                {
                    requestStream = new MemoryStream();
                    binaryWriter = new BinaryWriter(requestStream);

                    /*
                     * -> ECH\BYTE\CHARS{BYTEVALUELENGTH}
                     */

                    string echoContent =
                        Guid.NewGuid().ToString();
                    
                    binaryWriter.Write("ECH".ToCharArray());
                    binaryWriter.Write((byte)echoContent.Length);
                    binaryWriter.Write(echoContent.ToCharArray());
                    binaryWriter.Flush();

                    long requestId =
                        this._RequestHandler?.MakeRequest(((MemoryStream) requestStream).ToArray()) ?? -1;
                    if (requestId == -1) throw new Exceptions.ExternalCommunicationException();
                    
                    byte[] responseBytes =
                        this._ResponseHandler.WaitForMessage(requestId);

                    this.ApproveEcho(echoContent, responseBytes);
                    
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
                catch (Exceptions.ExternalCommunicationException)
                {
                    this.Reset();
                    return;
                }
                finally
                {
                    binaryWriter?.Close();
                    requestStream?.Close();
                }
            } while (true);
        }

        private void ApproveEcho(string echoContent, byte[] responseBytes)
        {
            if (responseBytes == null)
                return;
            
            BinaryReader binaryReader = null;
            Stream contentStream = null;

            try
            {
                contentStream =
                    new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader =
                    new BinaryReader(contentStream);

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}
                 */

                byte remoteResult =
                    binaryReader.ReadByte();

                switch (remoteResult)
                {
                    case 0:
                        byte echoMessageLength = binaryReader.ReadByte();
                        string echoMessage =
                            new string(binaryReader.ReadChars(echoMessageLength));

                        if (string.CompareOrdinal(echoContent, echoMessage) != 0)
                            throw new Exceptions.DssEchoMatchException();

                        break;
                    default:
                        throw new Exceptions.DssCommandException();
                }
            }
            finally
            {
                binaryReader?.Close();
                contentStream?.Close();
            }
        }

        // Always returns false
        public bool Reserve(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject)
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
                    this._RequestHandler?.MakeRequest(((MemoryStream) requestStream).ToArray()) ?? -1;
                if (requestId == -1) throw new Exceptions.ExternalCommunicationException();

                byte[] responseBytes =
                    this._ResponseHandler.WaitForMessage(requestId);

                this.ParseReservation(responseBytes, out reservationObject);

                return false;
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

        private void ParseReservation(byte[] responseBytes, out Basics.Dss.IDss reservationObject)
        {
            reservationObject = null;

            if (responseBytes == null)
                return;

            BinaryReader binaryReader = null;
            Stream contentStream = null;

            try
            {
                contentStream =
                    new MemoryStream(responseBytes, 0, responseBytes.Length, false);
                binaryReader =
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
                    new DateTime(binaryReader.ReadInt64(), DateTimeKind.Utc);

                reservationObject = new Service(ref this._RequestHandler, ref this._ResponseHandler, reservationId,
                    reusing == 1, expireDate);
            }
            finally
            {
                binaryReader?.Close();
                contentStream?.Close();
            }
        }
    }
}
