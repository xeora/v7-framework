using System;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.DSS
{
    public class RequestHandler
    {
        private TcpClient _DSSServiceClient;

        private long _LastRequestID = 0;
        private object _StreamLock;

        public RequestHandler(ref TcpClient dssServiceClient)
        {
            this._DSSServiceClient = dssServiceClient;

            this._LastRequestID = 0;
            this._StreamLock = new object();
        }

        private long PrepareRequest(ref byte[] requestBytes)
        {
            this._LastRequestID++;

            long contentLength = requestBytes.Length;
            long head = this._LastRequestID << 24;
            head |= contentLength;

            byte[] headBytes = BitConverter.GetBytes(head);

            byte[] newRequestBytes = new byte[headBytes.Length + requestBytes.Length];
            Array.Copy(headBytes, 0, newRequestBytes, 0, headBytes.Length);
            Array.Copy(requestBytes, 0, newRequestBytes, headBytes.Length, requestBytes.Length);

            requestBytes = newRequestBytes;

            return this._LastRequestID;
        }

        public long MakeRequest(byte[] requestBytes)
        {
            Monitor.Enter(this._StreamLock);
            try
            {
                // PrepareRequest always under lock!
                long requestID = 
                    this.PrepareRequest(ref requestBytes);

                this._DSSServiceClient.GetStream().Write(requestBytes, 0, requestBytes.Length);

                return requestID;
            }
            catch
            {
                throw new ExternalCommunicationException();
            }
            finally
            {
                Monitor.Exit(this._StreamLock);
            }
        }
    }
}
