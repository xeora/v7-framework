using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Xeora.Web.Service.Session
{
    public class ExternalManager : IHttpSessionManager
    {
        private IPEndPoint _ServiceEndPoint;
        private short _SessionTimeout = 20;

        public ExternalManager(IPEndPoint serviceEndPoint, short sessionTimeout)
        {
            this._ServiceEndPoint = serviceEndPoint;
            this._SessionTimeout = sessionTimeout;
        }

        public void Acquire(IPAddress remoteIP, string sessionID, out Basics.Session.IHttpSession sessionObject)
        {
            TcpClient tcpClient;
            this.MakeConnection(out tcpClient);

            this.SendAcquireRequest(ref tcpClient, remoteIP, sessionID);
            this.ParseSessionResponse(ref tcpClient, out sessionObject);
        }

        private void MakeConnection(out TcpClient tcpClient)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(this._ServiceEndPoint);

                if (!tcpClient.Connected)
                    throw new SocketException();
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
        }

        private void SendAcquireRequest(ref TcpClient tcpClient, IPAddress remoteIP, string sessionID)
        {
            BinaryWriter binaryWriter = null;
            Stream requestStream = null;

            try
            {
                requestStream = new MemoryStream();
                binaryWriter = new BinaryWriter(requestStream);

                /*
                 * -> ACQ\SHORT\INT\0
                 * -> ACQ\SHORT\INT\BYTE\CHARS{BYTEVALUELENGTH}\0
                 */

                binaryWriter.Write("ACQ".ToCharArray());
                binaryWriter.Write(this._SessionTimeout);
                binaryWriter.Write(remoteIP.GetAddressBytes());
                binaryWriter.Write((byte)sessionID.Length);
                binaryWriter.Write(sessionID.ToCharArray());
                binaryWriter.Write((byte)0);
                binaryWriter.Flush();

                requestStream.Seek(0, SeekOrigin.Begin);
                requestStream.CopyTo(tcpClient.GetStream());
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
        }

        private void ParseSessionResponse(ref TcpClient tcpClient, out Basics.Session.IHttpSession sessionObject)
        {
            BinaryReader binaryReader = null;

            try
            {
                binaryReader = new BinaryReader(tcpClient.GetStream());

                /*
                 * <- \BYTE\BYTE\CHARS{BYTEVALUELENGTH}\LONG\0
                 * <- \BYTE\0
                 */

                if (binaryReader.ReadByte() == 2)
                {
                    // Consume Terminator
                    binaryReader.ReadByte();

                    throw new SessionCreationException();
                }

                byte sessionIDLength = binaryReader.ReadByte();
                string sessionID = 
                    new string(binaryReader.ReadChars(sessionIDLength));
                DateTime expireDate =
                    new DateTime(binaryReader.ReadInt64());

                // Consume Terminator
                binaryReader.ReadByte();

                sessionObject = new ExternalSession(ref tcpClient, sessionID, expireDate);
            }
            catch (SessionCreationException)
            {
                throw;
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
        }

        public void Complete(ref Basics.Session.IHttpSession sessionObject) =>
            ((IHttpSessionService)sessionObject).Complete();
    }
}
