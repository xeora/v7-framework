using System;
using System.Net.Sockets;
using System.Threading;

namespace Xeora.Web.Service.Dss.External
{
    public class RequestHandler
    {
        private readonly TcpClient _DssServiceClient;

        private long _LastRequestId;
        private readonly object _StreamLock;

        public RequestHandler(ref TcpClient dssServiceClient)
        {
            this._DssServiceClient = dssServiceClient;

            this._LastRequestId = 0;
            this._StreamLock = new object();
        }

        private long PrepareRequest(ref byte[] requestBytes)
        {
            this._LastRequestId++;

            long contentLength = requestBytes.Length;
            long head = this._LastRequestId << 24;
            head |= contentLength;

            byte[] headBytes = BitConverter.GetBytes(head);

            byte[] newRequestBytes = 
                new byte[headBytes.Length + requestBytes.Length];
            Array.Copy(headBytes, 0, newRequestBytes, 0, headBytes.Length);
            Array.Copy(requestBytes, 0, newRequestBytes, headBytes.Length, requestBytes.Length);

            requestBytes = newRequestBytes;

            return this._LastRequestId;
        }

        public long MakeRequest(byte[] requestBytes)
        {
            if (this._DssServiceClient == null || !this._DssServiceClient.Client.Connected) return -1;
            
            Monitor.Enter(this._StreamLock);
            try
            {
                // PrepareRequest always under lock!
                long requestId = 
                    this.PrepareRequest(ref requestBytes);

                this._DssServiceClient.GetStream().Write(requestBytes, 0, requestBytes.Length);

                return requestId;
            }
            catch
            {
                throw new Exceptions.ExternalCommunicationException();
            }
            finally
            {
                Monitor.Exit(this._StreamLock);
            }
        }
    }
}
