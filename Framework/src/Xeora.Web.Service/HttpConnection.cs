using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xeora.Web.Service
{
    public class HttpConnection
    {
        private TcpClient _RemoteClient;

        public HttpConnection(ref TcpClient remoteClient) =>
            this._RemoteClient = remoteClient;

        public void HandleAsync() =>
            this.HandleClientAsync();

        private async void HandleClientAsync()
        {
            IPAddress remoteAddr = ((IPEndPoint)this._RemoteClient.Client.RemoteEndPoint).Address;

            // TODO: SSL handling should be done!
            NetworkStream remoteStream = this._RemoteClient.GetStream();

            // If reads create problems and put the loop to infinite. drop the connection.
            // that's why, 5 seconds timeout should be set to remoteStream
            // No need to put timeout to write operation because xeora will handle connection state
            remoteStream.ReadTimeout = 5 * 1000;

            ClientState clientState = new ClientState(remoteAddr, ref remoteStream);
            await Task.Run(() => clientState.Handle());
            clientState.Dispose();

            this._RemoteClient.Close();
        }
    }
}
